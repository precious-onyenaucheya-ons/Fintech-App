using Payment.Core.DTOs;
using Payment.Core.DTOs.PaystackDtos.FundAccountDto;
using Payment.Core.DTOs.TransactionsDto;
using Payment.Core.Interfaces;
using Payment.Domain.Models;
using Serilog;
using System.Net;
using Payment.Core.Utilities;
using AutoMapper;
using Payment.Domain.Enums;
using Payment.Core.DTOs.PaystackDtos.TransferDto;
using Payment.Core.Utilities.Settings;
using Payment.Core.DTOs.BeneficiaryDto;

namespace Payment.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayStackPaymentHandler _paymentHandler;
        private readonly ILogger _logger;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IBankAccountService _bankAccountService;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork,
            IPayStackPaymentHandler payStackPaymentHandler, IMapper mapper, ILogger logger, IBankAccountService bankAccountService, ApplicationSettings applicationSettings)
        {
            _unitOfWork = unitOfWork;
            _paymentHandler = payStackPaymentHandler;
            _logger = logger;
            _applicationSettings = applicationSettings;
            _bankAccountService = bankAccountService;
            _mapper = mapper;
        }

        public async Task<ResponseDto<string>> Transfer(TransferRequestDto request, string walletId) 
        {
            try
            {
                var wallet = await _unitOfWork.Wallets.Get(walletId);

                if (wallet == null)
                {
                    _logger.Information($"Couldn't find wallet with Id = {walletId}");
                    return ResponseDto<string>.Fail($"Wallet with Id = {walletId} does not exist.", (int)HttpStatusCode.NotFound);
                }

                if (!BCrypt.Net.BCrypt.Verify(request.TransferPin, wallet.Pin))
                {
                    _logger.Information($"Unable to initiate transfer because of invalid pin");
                    return ResponseDto<string>.Fail("Invalid Pin, Kindly enter a correct PIN", (int)HttpStatusCode.Unauthorized);
                }

                _logger.Information("Attempting to fetch List of Transfer Recipients");
                var recipients = await _paymentHandler.GetListOfTransferRecipients();
                if (recipients.status == false)
                {
                    _logger.Information("Unable to fetch list of recipients due to service unavailability");
                    return ResponseDto<string>.Fail(recipients.message, (int)HttpStatusCode.ServiceUnavailable);
                }

                if (wallet.Balance - request.Amount < 0 || request.Amount < 0)
                {
                    _logger.Information("Failed Transfer Initiation due to Insufficient Account Balance of Amount Input");
                    return ResponseDto<string>.Fail("Insufficient Fund", (int)HttpStatusCode.BadRequest);
                }

                var recipient = recipients.data.FirstOrDefault(x => x.details.account_number == request.AccountNumber);

                var balance = wallet.Balance - request.Amount;
                if (recipient == null)
                {
                    var newRecipient = await _paymentHandler.CreateTransferRecipient(request);

                    TransferDataRequestDto transferRequest = TransferRequest(newRecipient.data.recipient_code, request.Amount);

                    var InitiateTransfer = await _paymentHandler.InitiateTransfer(transferRequest);

                    if (!InitiateTransfer.Status)
                    {
                        _logger.Information("Unable to initialize transfer due to service unavailability");
                        return ResponseDto<string>.Fail(InitiateTransfer.Message, (int)HttpStatusCode.ServiceUnavailable);
                    }
                    Transaction transaction = Transaction(InitiateTransfer.Data.reference, InitiateTransfer.Data.Amount,
                        walletId, request.Description, $"Transfer from {wallet.Name}", InitiateTransfer.Data.TransferCode);

                    if (request.Check == true)
                    {
                        var response = _mapper.Map<BeneficiaryRequestDto>(request);
                        await _bankAccountService.AddBeneficiaryAsync(response, walletId);
                    }

                    await _unitOfWork.Transactions.AddAsync(transaction);
                    wallet.Balance = balance;
                    _unitOfWork.Wallets.Update(wallet);
                    await _unitOfWork.Save();

                    await VerifyTransferAsync(InitiateTransfer.Data.reference);

                    _logger.Information("Transfer Initiated when specific recipient from list is null");
                    return ResponseDto<string>.Success("Transaction processing.", $"Transfer from {wallet.Name}");
                }

                TransferDataRequestDto recipientTransfer = TransferRequest(recipient.recipient_code, request.Amount);
                var InitiateRecipientTransfer = await _paymentHandler.InitiateTransfer(recipientTransfer);

                if (!InitiateRecipientTransfer.Status)
                {
                    _logger.Information("Unable to initialize transfer due to service unavailability");
                    return ResponseDto<string>.Fail(InitiateRecipientTransfer.Message, (int)HttpStatusCode.ServiceUnavailable);
                }

                Transaction Wallettransaction = Transaction(InitiateRecipientTransfer.Data.reference, InitiateRecipientTransfer.Data.Amount, walletId,
                    request.Description, $"Transfer from {wallet.Name}", InitiateRecipientTransfer.Data.TransferCode);
                await _unitOfWork.Transactions.AddAsync(Wallettransaction);
                wallet.Balance = balance;

                if (request.Check == true)
                {
                    var response = _mapper.Map<BeneficiaryRequestDto>(request);
                    await _bankAccountService.AddBeneficiaryAsync(response, walletId);
                }

                _unitOfWork.Wallets.Update(wallet);
                await _unitOfWork.Save();

                await VerifyTransferAsync(InitiateRecipientTransfer.Data.reference);

                _logger.Information("Transfer Initiated when specific recipient from list is null");
                return ResponseDto<string>.Success("Transaction processing.", $"Transfer from {wallet.Name}");
            }
            catch (Exception ex)
            {
                _logger.Information($"Unable to transfer: {ex.Message}");
                return ResponseDto<string>.Fail($"Unable to transfer:{ex.Message}", (int)HttpStatusCode.PaymentRequired);
            }
        }

        /// <summary>
        /// Gets all the transaction carried out in a particular wallet and return is in pages
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ResponseDto<PaginationResult<IEnumerable<TransactionHistoryResponseDto>>>> GetTransactionHistoryAsync(string walletId, int pageNumber)
        {
            var transactions = _unitOfWork.Transactions.GetTransactionHistory(walletId).OrderByDescending(t => t.CreatedAt);

            var paginatedResult = await Paginator.PaginationAsync<Transaction, TransactionHistoryResponseDto>
                    (transactions, _applicationSettings.PageSize, pageNumber, _mapper);
            return ResponseDto<PaginationResult<IEnumerable<TransactionHistoryResponseDto>>>.Success
                ("Successful", paginatedResult, (int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Verifies if a transaction status is successful or not. Returns true if successful
        /// </summary>
        /// <param name="transferReference"></param>
        /// <returns>Returns true if transaction status is successful</returns>
        public async Task<bool> VerifyTransferAsync(string transferReference)
        {
            _logger.Information($"Attempting to retrieve transaction by transaction reference: {transferReference}");
            var transaction = await _unitOfWork.Transactions.GetAsync(x => x.Reference == transferReference);

            if (transaction == null)
            {
                _logger.Information($"Unable to retrieve transaction. Because transaction reference: {transferReference} does not exist");
                return false;
            }
            _logger.Information($"Transaction with reference: {transferReference} sucessfully retrieved");

            var response = await _paymentHandler.VerifyTransfer(transferReference);
            _logger.Information($"Verifying transaction with reference: {transferReference}");

            if (response)
            {
                transaction.Status = TransactionStatus.Successful;
                transaction.UpdatedAt = DateTime.Now;
                _logger.Information($"Updating transaction: {transferReference} status to successful");
                _logger.Information($"Transaction: {transferReference} saved");
                await _unitOfWork.Save();
                return true;
            }
            return false;
        }

        private TransferDataRequestDto TransferRequest(string recipientCode, decimal Amount)
        {
            int amount = Convert.ToInt32(Amount);

            return new TransferDataRequestDto()
            {
                RecipientCode = recipientCode,
                Amount = amount,
            };
        }


        private Transaction Transaction(string reference, decimal amount, string walletId, string description, string narration, string transferCode)
        {
            return new Transaction()
            {
                Reference = reference,
                Amount = amount,
                IsInternal = false,
                WalletId = walletId,
                Description = description,
                Type = TransactionType.Withdrawal,
                Status = TransactionStatus.Pending,
                Narration = narration,
                TransactionCode = transferCode

            };
        }
    }
}
