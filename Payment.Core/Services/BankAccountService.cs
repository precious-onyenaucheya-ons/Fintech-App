using AutoMapper;
using Payment.Core.DTOs;
using Payment.Core.DTOs.BeneficiaryDto;
using Payment.Core.DTOs.PaystackDtos.BankDtos;
using Payment.Core.Interfaces;
using Payment.Core.Utilities;
using Payment.Core.Utilities.Settings;
using Payment.Domain.Models;
using Serilog;
using System.Net;

namespace Payment.Core.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayStackPaymentHandler _paymentHandler;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationSettings _applicationSettings;

        public BankAccountService(IUnitOfWork unitOfWork, IPayStackPaymentHandler paymentHandler, ILogger logger,
            IMapper mapper, ApplicationSettings applicationSettings)
        {
            _unitOfWork = unitOfWork;
            _paymentHandler = paymentHandler;
            _logger = logger;
            _mapper = mapper;
            _applicationSettings = applicationSettings;
        }

        /// <summary>
        /// returns the responseDto of created Successful if Beneficiary is added successfully
        /// </summary>
        /// <param name="request"></param>
        /// <param name="walletId"></param>
        /// <returns>Beneficiary Added if Successful</returns>       
        public async Task<ResponseDto<string>> AddBeneficiaryAsync(BeneficiaryRequestDto request, string walletId)
        {
            try
            {
                _logger.Information("Fetching List of Transfer Recipient");
                var recipients = await _paymentHandler.GetListOfTransferRecipients();

                if (recipients.status == false)
                {
                    _logger.Information("Unable to fetch list of recipients due to service unavailability");
                    return ResponseDto<string>.Fail(recipients.message, (int)HttpStatusCode.ServiceUnavailable);
                }

                var beneficiary = await _unitOfWork.BankAccounts.GetBeneficiaryById(walletId, request.AccountNumber);
                if (beneficiary != null)
                {
                    _logger.Information("Unable to add beneficiary becasue beneficiary already exist");
                    return ResponseDto<string>.Fail("Beneficiary already exist", (int)HttpStatusCode.BadRequest);
                }

                var recipient = recipients.data.Where(x => x.details.account_number == request.AccountNumber).FirstOrDefault();
                if (recipient == null)
                {
                    _logger.Information("recipient does not exist, attempting to create one");

                    var newRecipient = await _paymentHandler.CreateBeneficiary(request);
                    if (!newRecipient.status)
                    {
                        _logger.Information("An Error occured while creating the recipient");
                        return ResponseDto<string>.Fail(newRecipient.message, (int)HttpStatusCode.ServiceUnavailable);
                    }

                    var bank = await _unitOfWork.Banks.GetBankByPaystackIdAsync(request.BeneficiaryBank.PaystackBankId);
                    if (bank == null)
                    {
                        bank = _mapper.Map<Bank>(request.BeneficiaryBank);
                        await _unitOfWork.Banks.AddAsync(bank);
                    }

                    BankAccount beneficiaryAccount = CreateBankAccount(request.AccountName, request.AccountNumber, newRecipient.data.recipient_code,
                       newRecipient.data.createdAt, newRecipient.data.updatedAt, walletId, bank.Id);

                    await _unitOfWork.BankAccounts.AddAsync(beneficiaryAccount);
                    await _unitOfWork.Save();

                    _logger.Information("Beneficiary Added when specific recipient from list is null");
                    return ResponseDto<string>.Success("Beneficiary Successfully Added", newRecipient.data.recipient_code);
                }

                var bank2 = await _unitOfWork.Banks.GetBankByPaystackIdAsync(request.BeneficiaryBank.PaystackBankId);
                if (bank2 == null)
                {
                    bank2 = _mapper.Map<Bank>(request.BeneficiaryBank);
                    await _unitOfWork.Banks.AddAsync(bank2);
                }

                BankAccount account = CreateBankAccount(request.AccountName, request.AccountNumber, recipient.recipient_code,
                       recipient.createdAt, recipient.updatedAt, walletId, bank2.Id);

                await _unitOfWork.BankAccounts.AddAsync(account);
                await _unitOfWork.Save();

                _logger.Information("Beneficiary Added when specific recipient from list is not null");
                return ResponseDto<string>.Success("Beneficiary Successfully Added", recipient.recipient_code);
            }
            catch(Exception ex)
            {
                _logger.Error($"Could not add beneficiary: {ex.Message}");
                return ResponseDto<string>.Fail($"could not add beneficiary : {ex.Message}", (int)HttpStatusCode.ServiceUnavailable);
                
            }
           
        }

        public async Task<ResponseDto<VerifyAccountResponseDto>> VerifyAccountNumber(VerifyAccountRequestDto request)
        {
            try
            {
                var response = await _paymentHandler.VerifyAccountNumber(request);

                _logger.Information("verifying account Number");
                if (!response.Status)
                {
                    return ResponseDto<VerifyAccountResponseDto>.Fail(response.Message, (int)HttpStatusCode.BadRequest);
                }

                _logger.Information("verified");
                return ResponseDto<VerifyAccountResponseDto>.Success(response.Message, response.Data, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.Error($"Could not add beneficiary: {ex.Message}");
                return ResponseDto<VerifyAccountResponseDto>.Fail($"verification failde : {ex.Message}", (int)HttpStatusCode.ServiceUnavailable);

            }

        }

        public async Task<ResponseDto<PaginationResult<IEnumerable<BeneficiaryListResponseDto>>>> 
            GetAllBeneficiariesAsync(int pageNumber, string walletId)
        {
            var wallet = await _unitOfWork.Wallets.Get(walletId);

            if (wallet == null)
            {
                return ResponseDto<PaginationResult<IEnumerable<BeneficiaryListResponseDto>>>.Fail
                   ("UnSuccessfull", (int)HttpStatusCode.BadRequest);
            }

            var Beneficiaries = _unitOfWork.BankAccounts.GetAllBeneficiaryAsync(walletId);
            

            var paginatedResult = await Paginator.PaginationAsync<BankAccount, BeneficiaryListResponseDto>
                    (Beneficiaries, _applicationSettings.PageSize, pageNumber, _mapper);
            return ResponseDto<PaginationResult<IEnumerable<BeneficiaryListResponseDto>>>.Success
                ("Successful", paginatedResult, (int)HttpStatusCode.OK);
        }

        public async Task<ResponseDto<string>> DeleteBeneficiary(string accountNumber, string walletId)
        {
            try
            {
                var wallet = await _unitOfWork.Wallets.Get(walletId);

                if (wallet == null)
                {
                    _logger.Information("wallet is null");
                    return ResponseDto<string>.Fail("invalid walletId", (int)HttpStatusCode.BadRequest);
                }

                var beneficiaries = _unitOfWork.BankAccounts.GetAllBeneficiaryAsync(walletId);

                var beneficiary = beneficiaries.Where(x => x.AccountNumber == accountNumber).FirstOrDefault();
                if (beneficiary == null)
                {
                    _logger.Information($"beneficiary with {accountNumber} does not exist");
                    return ResponseDto<string>.Fail($"cannot delete {accountNumber}, does not exist", (int)HttpStatusCode.BadRequest);
                }

                _unitOfWork.BankAccounts.Delete(beneficiary);
                await _unitOfWork.Save();
                _logger.Information($"Beneficiary with with account number {beneficiary.AccountNumber}deleted");

                return ResponseDto<string>.Success("Deleted","Beneficiary successfully deleted", (int)HttpStatusCode.NoContent);
            }
            catch(Exception ex)
            {
                _logger.Error($"Could not delete beneficiary: {ex.Message}");
                throw;
            }           
        }

        private BankAccount CreateBankAccount(string accName, string accNumber, string rcpCode, DateTime createdAt, DateTime updatedAt,
          string walletId, string bankId, bool isInternal = false)
        {
            return new BankAccount()
            {
                AccountName = accName,
                AccountNumber = accNumber,
                RecipientCode = rcpCode,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                IsInternal = isInternal,
                WalletId = walletId,
                BankId = bankId,
            };
        }


    }
}
