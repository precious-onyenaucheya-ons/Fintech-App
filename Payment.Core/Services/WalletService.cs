using AutoMapper;
using FluentValidation;
using Payment.Core.DTOs;
using Payment.Core.DTOs.PaystackDtos.FundAccountDto;
using Payment.Core.DTOs.PaystackDtos.VirtualAccountDtos;
using Payment.Core.DTOs.TransactionsDto;
using Payment.Core.DTOs.WalletDtos;
using Payment.Core.Interfaces;
using Payment.Core.Utilities.Validators;
using Payment.Domain.Enums;
using Payment.Domain.Models;
using Paystack.Net.SDK.Models;
using Serilog;
using shortid;
using shortid.Configuration;
using System.Net;
using System.Transactions;


namespace Payment.Core.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IVirtualAccountService _virtualAccountService;
        private readonly IPayStackPaymentHandler _payStackPaymentHandler;
        private readonly ITransactionService _transactionService;

        public WalletService(IUnitOfWork unitOfWork, IMapper mapper, IPayStackPaymentHandler payStackPaymentHandler,
            ILogger logger, IVirtualAccountService virtualAccountService, ITransactionService transactionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _virtualAccountService = virtualAccountService;
            _payStackPaymentHandler = payStackPaymentHandler;
            _transactionService = transactionService;
        }

        public async Task<ResponseDto<WalletResponseDto>> GetWalletByIdAsync(string id)
        {
            _logger.Information($"Attempting to fetch record for {id}");
            var wallet = await _unitOfWork.Wallets.Get(id);
            if (wallet == null)
            {
                _logger.Information($"Wallet with id = {id} does not exist in record");
                return ResponseDto<WalletResponseDto>.Fail($"Wallet with id = {id} does not exist in record");
            }

            _logger.Information($"Wallet with Id = {id}, retrieved successfully");
            var walletDto = _mapper.Map<WalletResponseDto>(wallet);
            return ResponseDto<WalletResponseDto>.Success("Wallet successfully retrieved", walletDto);
        }

        public async Task<ResponseDto<WalletResponseDto>> GetWalletByUserIdAsync(string userId)
        {
            _logger.Information($"Attempting to fetch wallet for {userId}");
            var wallet = await _unitOfWork.Wallets.GetUserWallet(userId);
            if (wallet == null)
            {
                _logger.Information($"Wallet with UserId = {userId} does not exist in records");
                return ResponseDto<WalletResponseDto>.Fail($"Wallet with UserId = {userId} does not exist in records");
            }

            _logger.Information($"Wallet with UserId = {userId} retrieved successfully");
            var walletDto = _mapper.Map<WalletResponseDto>(wallet);
            return ResponseDto<WalletResponseDto>.Success("Wallet successfully retrieved", walletDto);
        }

        public async Task<ResponseDto<CreateWalletResponse>> CreateWalletAsync(CreateWalletRequest walletRequestDto)
        {
            Wallet wallet;

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                // register our user as paystack customer
                var customerResponse = await _payStackPaymentHandler.CreateCustomer(walletRequestDto);
                if (!customerResponse.status)
                {
                    _logger.Information($"Could not create Paystack customer account for user " +
                        $"{walletRequestDto.UserId} : {customerResponse.message}");
                    return ResponseDto<CreateWalletResponse>
                        .Fail($"Could not create Paystack customer account for user: {customerResponse.message}", (int)HttpStatusCode.BadRequest);
                }

                // create wallet
                wallet = _mapper.Map<Wallet>(walletRequestDto);
                wallet.PaystackCustomerCode = customerResponse.data.customer_code;
                wallet.Pin = BCrypt.Net.BCrypt.HashPassword(walletRequestDto.Pin);
                wallet.Code = ShortId.Generate(new GenerationOptions(length: 8));

                await _unitOfWork.Wallets.AddAsync(wallet);

                // create virtual account
                var virtualAccountRequest = new CreateVirtualAccountRequest()
                {
                    PaystackCustomerCode = wallet.PaystackCustomerCode,
                    WalletId = wallet.Id,
                };

                await _virtualAccountService.CreateVirtualAccount(virtualAccountRequest);

                // save changes
                await _unitOfWork.Save();
                _logger.Information($"Successfully added wallet {wallet.Id} to the database.");
                scope.Complete();
            }
            catch (Exception ex)
            {
                _logger.Error($"Could not create wallet: {ex.Message}");
                scope.Dispose();
                return ResponseDto<CreateWalletResponse>.Fail($"Could not create wallet: {ex.Message}", (int)HttpStatusCode.BadRequest);
            }

            var walletResponse = _mapper.Map<CreateWalletResponse>(wallet);

            return ResponseDto<CreateWalletResponse>.Success("Wallet successfully created", walletResponse, (int)HttpStatusCode.Created);
        }

        public async Task<ResponseDto<string>> LocalTransferAsync(string walletId, LocalTransferRequestDto request)
        {
            var wallet = await _unitOfWork.Wallets.Get(walletId);
            var destinationWallet = await _unitOfWork.Wallets.Get(request.DestinationWalletCode);

            // make sure both wallets exist

            if (wallet == null)
            {
                _logger.Information("Local transfer error: source wallet does not exist");
                return ResponseDto<string>.Fail("Source wallet does not exist", (int)HttpStatusCode.BadRequest);
            }

            if (destinationWallet == null)
            {
                _logger.Information("Local transfer error: destination wallet does not exist");
                return ResponseDto<string>.Fail("Destination wallet does not exist", (int)HttpStatusCode.BadRequest);
            }

            // make sure amount is valid
            if (request.Amount <= 0)
            {
                _logger.Information("Unable to process transaction, amount is invalid");
                return ResponseDto<string>.Fail("Amount entered must be greater than 0", (int)HttpStatusCode.BadRequest);
            }

            // ensure PIN is valid
            if (!BCrypt.Net.BCrypt.Verify(request.TransactionPIN, wallet.Pin))
            {
                _logger.Information("Unable to process transaction, invalid wallet PIN");
                return ResponseDto<string>.Fail("Invalid wallet PIN", (int)HttpStatusCode.BadRequest);
            }

            // ensure wallet balance is sufficient
            if (wallet.Balance < request.Amount)
            {
                _logger.Information("Could not process transaction, Insufficient funds");
                return ResponseDto<string>.Fail("Insufficient funds", (int)HttpStatusCode.BadRequest);
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                _logger.Information($"Initiating transfer transaction to debit wallet {wallet.Id} and credit wallet {destinationWallet.Id}");

                // initiate transactions and debit/credit
                wallet.Balance -= request.Amount;

                var sourceWalletTransaction = InstantiateTransaction(request.Amount, "",
                    Domain.Enums.TransactionStatus.Successful, null, TransactionType.Withdrawal,
                    request.Description, $"Transfer from {wallet.Name} to {destinationWallet.Name}", wallet.Id);

                destinationWallet.Balance += request.Amount;

                var destinationWalletTransaction = InstantiateTransaction(request.Amount, "",
                    Domain.Enums.TransactionStatus.Successful, null, TransactionType.Funding, request.Description,
                    $"Transfer from {wallet.Name} to {destinationWallet.Name}", destinationWallet.Id);

                await _unitOfWork.Transactions.AddAsync(sourceWalletTransaction);
                await _unitOfWork.Transactions.AddAsync(destinationWalletTransaction);

                wallet.UpdatedAt = DateTime.UtcNow;
                destinationWallet.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Wallets.Update(wallet);
                _unitOfWork.Wallets.Update(destinationWallet);

                await _unitOfWork.Save();
                _logger.Information($"Successfuly transferred {request.Amount} from wallet {wallet.Id} to wallet {destinationWallet.Id}");
                scope.Complete();

                return ResponseDto<string>.Success("Success",
                    $"Successfully transferred {request.Amount} from wallet {wallet.Id} to wallet {destinationWallet.Id}");

            }
            catch (Exception ex)
            {
                _logger.Error($"Local transfer error: {ex.Message}");
                await _unitOfWork.Rollback();
                scope.Dispose();
                return ResponseDto<string>.Fail("Error processing transfer", (int)HttpStatusCode.ServiceUnavailable);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        public async Task<ResponseDto<PaymentInitalizationResponseModel>> InitializeFundWallet(FundWalletRequest request, string walletId)
        {
            // log info
            var wallet = await _unitOfWork.Wallets.Get(walletId);
            if (wallet == null)
            {
                // Log info
                _logger.Information($"Wallet with {walletId} is null");
                return ResponseDto<PaymentInitalizationResponseModel>.Fail($"Wallet with id = {walletId} does not exist", (int)HttpStatusCode.NotFound);
            }

            // log info
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var paymentResponse = await _payStackPaymentHandler.InitializePayment(request);

                if (paymentResponse.status == false)
                {
                    _logger.Information($"Unable to initiate transaction to wallet with the Id of {walletId}");
                    return ResponseDto<PaymentInitalizationResponseModel>.Fail(paymentResponse.message, (int)HttpStatusCode.PaymentRequired);
                }

                Domain.Models.Transaction transaction = new()
                {
                    Amount = request.Amount,
                    Reference = paymentResponse.data.reference,
                    Status = Domain.Enums.TransactionStatus.Pending,
                    Type = Domain.Enums.TransactionType.Funding,
                    IsInternal = false,
                    Description = request.Description ?? "",
                    Narration = $"Wallet funding by {wallet.Name}",
                    WalletId = walletId
                };

                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.Save();
                _logger.Information("Inintialization of payment completed");
                scope.Complete();
                _logger.Information("Transaction initiated ");
                return ResponseDto<PaymentInitalizationResponseModel>.Success("Transaction initiated", paymentResponse, (int)HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                _logger.Information("Unable to initiate Transaction");
                return ResponseDto<PaymentInitalizationResponseModel>.Fail($"Unable to initialize payment:{ex.Message}", (int)HttpStatusCode.PaymentRequired);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="walletId"></param>
        /// <returns></returns>
        public async Task<ResponseDto<string>> FundWalletVerificationAsync(VerifyTransactionRequestDto request, string walletId)
        {
            if (string.IsNullOrWhiteSpace(request.Reference))
            {
                _logger.Information($"Wallet with {walletId} is null");
                return ResponseDto<string>.Fail($"Wallet with id = {walletId} does not exist", (int)HttpStatusCode.NotFound);
            }

            var wallet = await _unitOfWork.Wallets.Get(walletId);
            var transaction = await _unitOfWork.Transactions.GetTransactionByReference(request.Reference);

            if (transaction == null)
            {
                return ResponseDto<string>.Fail($"Transaction does not exist", (int)HttpStatusCode.NotFound);
            }

            if (transaction.WalletId != wallet.Id)
            {
                _logger.Information($"reference and walletId does not match");
                return ResponseDto<string>.Fail($"Transaction is not for this wallet", (int)HttpStatusCode.NotFound);
            }

            if (string.IsNullOrWhiteSpace(walletId))
            {
                // Log info
                _logger.Information($"Wallet with {walletId} is null");
                return ResponseDto<string>.Fail($"Wallet with id = {walletId} does not exist", (int)HttpStatusCode.NotFound);
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var paymentResponse = await _payStackPaymentHandler.VerifyPayment(request.Reference);

                if (paymentResponse.data.status != "success")
                {
                    _logger.Information($"Unable to complete payment to wallet with the Id of {walletId}");
                    return ResponseDto<string>.Fail(paymentResponse.message, (int)HttpStatusCode.PaymentRequired);
                }

                wallet.Balance += paymentResponse.data.amount / 100;

                transaction.Status = Domain.Enums.TransactionStatus.Successful;
                transaction.UpdatedAt = DateTime.UtcNow;
                wallet.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Wallets.Update(wallet);
                _unitOfWork.Transactions.Update(transaction);
                await _unitOfWork.Save();
                _logger.Information("Payment completed");
                scope.Complete();
                return ResponseDto<string>.Success(paymentResponse.status.ToString(), "Payment completed", (int)HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                _logger.Information("Unable to complete payment");
                scope.Dispose();
                return ResponseDto<string>.Fail($"Unable to complete payment:{ex.Message}", (int)HttpStatusCode.PaymentRequired);
            }
        }

        private static Domain.Models.Transaction InstantiateTransaction(decimal amount, string reference, Domain.Enums.TransactionStatus status, string? transCode, TransactionType transactionType, string description, string narration, string walletId)
        {
            return new Domain.Models.Transaction
            {
                Amount = amount,
                Reference = reference,
                Narration = narration,
                TransactionCode = transCode,
                Status = status,
                Type = transactionType,
                IsInternal = true,
                Description = description,
                WalletId = walletId ?? string.Empty
            };
        }

    }
}
