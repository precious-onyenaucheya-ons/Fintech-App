using Microsoft.Extensions.Configuration;
using Payment.Core.DTOs;
using Payment.Core.DTOs.BeneficiaryDto;
using Payment.Core.DTOs.PaystackDtos;
using Payment.Core.DTOs.PaystackDtos.BankDtos;
using Payment.Core.DTOs.PaystackDtos.FundAccountDto;
using Payment.Core.DTOs.PaystackDtos.TransferDto;
using Payment.Core.DTOs.PaystackDtos.VirtualAccountDtos;
using Payment.Core.DTOs.TransactionsDto;
using Payment.Core.DTOs.VirtualAccountDtos.TransactionsDto;
using Payment.Core.DTOs.WalletDtos;
using Payment.Core.Interfaces;
using Paystack.Net.SDK;
using Paystack.Net.SDK.Models;
using Paystack.Net.SDK.Models.Customers;
using Paystack.Net.SDK.Models.Transfers.Recipient;

namespace Payment.Core.Utilities.PaymentGatewaySettings
{
    public class PayStackPaymentHandler : IPayStackPaymentHandler
    {

        private readonly IConfiguration _config;
        private readonly IHttpClientService _httpClientService;
        private readonly string secret;
        private readonly string _baseUrl;
        private readonly string _fundingCallBackBaseUrl;
        public PayStackApi PayStack { get; set; }
        public PayStackPaymentHandler(IConfiguration config, IHttpClientService httpClientService)
        {
            _config = config;
            secret = _config["PaystackSettings:SecretKey"];
            _baseUrl = _config["PaystackSettings:BaseUrl"];
            PayStack = new PayStackApi(secret);
            _httpClientService = httpClientService;
            _fundingCallBackBaseUrl = _config["PaystackSettings:FundingCallBackBaseUrl"];
        }

        #region Transactions
        /// <summary>
        /// Initaize transaction on paystack
        /// </summary>
        /// <param name="request"></param>
        /// <returns>PaymentInitalizationResponseModel</returns>
        public async Task<PaymentInitalizationResponseModel> InitializePayment(FundWalletRequest request)
        {
            return await PayStack.Transactions.InitializeTransaction(request.Email, request.Amount * 100, callbackUrl: $"{_fundingCallBackBaseUrl}/dashboard");
        }

        /// <summary>
        /// Verify transaction from paystack
        /// </summary>
        /// <param name="transactionRef"></param>
        /// <returns>TransactionResponseModel</returns>
        public async Task<TransactionResponseModel> VerifyPayment(string transactionRef)
        {
            return await PayStack.Transactions.VerifyTransaction(transactionRef);
        }
        #endregion

        #region Transfers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TransferRecipientModel> CreateTransferRecipient(TransferRequestDto request)
        {
            return await PayStack.Transfers.CreateTransferRecipient(request.RecipientType, request.AccountName, request.AccountNumber, request.BeneficiaryBank.BankCode, description: request.Description);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TransferRecipientModel> CreateBeneficiary(BeneficiaryRequestDto request)
        {
            return await PayStack.Transfers.CreateTransferRecipient(request.RecipientType, request.AccountName, request.AccountNumber, request.BeneficiaryBank.BankCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PaystackGenericResponseDto<VerifyAccountResponseDto>> VerifyAccountNumber(VerifyAccountRequestDto requestDto)
        {
        
            var url = $"{_baseUrl}/bank/resolve?account_number={requestDto.AccountNumber}&bank_code={requestDto.BankCode}";
            return await _httpClientService.GetRequestAsync<PaystackGenericResponseDto<VerifyAccountResponseDto>>(_baseUrl, url, secret);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<TransferRecipientListModel> GetListOfTransferRecipients()
        {
            return await PayStack.Transfers.ListTransferRecipients();
        }


        public async Task<PaystackGenericResponseDto<TransferResponseDto>> InitiateTransfer(TransferDataRequestDto requestDto)
        {
            var url = $"{_baseUrl}/transfer";
            return await _httpClientService.PostRequestAsync
                <TransferDataRequestDto, PaystackGenericResponseDto<TransferResponseDto>>
                (_baseUrl, url, requestDto, secret);
        }

        public async Task<bool> VerifyTransfer(string reference)
        {
            var url = $"{_baseUrl}/transfer/verify/{reference}";
            var response = await _httpClientService.GetRequestAsync
                <ResponseDto<TransferVerifyResponseDto>>
                (_baseUrl, url, secret);
            return response.Data.Status == "success";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public async Task<TransferInitiationModel> InitiateTransferToBeneficiary(BeneficiaryTansferRequestDto request, string recipientCode, string balance)
        //{
        //    return await PayStack.Transfers.InitiateTransfer(amount: Convert.ToInt32(request.Amount)*100, recipientCode, reason: request.Description);
        //}

        #endregion

        #region CreateCustomer
        /// <summary>
        /// Calls the Paystack API to create a customer account for the user
        /// </summary>
        /// <param name="request">Object containing parameters to be sent to the Paystack API</param>
        /// <returns>Returns a response based on the success or failure of the request</returns>
        public async Task<CustomerCreationResponse> CreateCustomer(CreateWalletRequest request)
        {
            return await PayStack.Customers.CreateCustomer(request.UserEmail, request.FirstName, request.LastName);
        }

        #endregion

        /// <summary>
        /// Calls the Paystack API to create a virtual account
        /// </summary>
        /// <param name="requestDto">Object containing parameters to be sent to the Paystack API</param>
        /// <returnsReturns a response based on the success or failure of the request></returns>
        public async Task<PaystackGenericResponseDto<CreateVirtualAccountResponse>> CreateVirtualAccount(CreateVirtualAccountRequest requestDto)
        {
            var url = $"{_baseUrl}/{Constants.CREATEVIRTUALACCOUNTURL}";
            return await _httpClientService.PostRequestAsync
                <CreateVirtualAccountRequest, PaystackGenericResponseDto<CreateVirtualAccountResponse>>
                (_baseUrl, url, requestDto, secret);
        }
        public async Task<ResponseDto<List<BankResponseDto>>> GetAllBanksAsync()
        {
            var url = $"{_baseUrl}/{Constants.LISTBANKSURL}";
            var response = await _httpClientService.GetRequestAsync<PaystackGenericResponseDto<List<BankResponseDto>>>(_baseUrl, url, secret);
            if (response.Status)
            {
                return ResponseDto<List<BankResponseDto>>.Success("successful", response.Data, 200);
            }
            return ResponseDto<List<BankResponseDto>>.Fail("fail", 404);
        }
    }
}
