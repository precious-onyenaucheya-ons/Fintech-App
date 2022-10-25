using Payment.Core.DTOs;
using Payment.Core.DTOs.BeneficiaryDto;
using Payment.Core.DTOs.PaystackDtos;
using Payment.Core.DTOs.PaystackDtos.BankDtos;
using Payment.Core.DTOs.PaystackDtos.FundAccountDto;
using Payment.Core.DTOs.PaystackDtos.TransferDto;
using Payment.Core.DTOs.PaystackDtos.VirtualAccountDtos;
using Payment.Core.DTOs.TransactionsDto;
using Payment.Core.DTOs.WalletDtos;
using Payment.Core.Utilities.PaymentGatewaySettings;
using Paystack.Net.SDK.Models;
using Paystack.Net.SDK.Models.Customers;
using Paystack.Net.SDK.Models.Transfers.Recipient;

namespace Payment.Core.Interfaces
{
    public interface IPayStackPaymentHandler
    {
        Task<PaymentInitalizationResponseModel> InitializePayment(FundWalletRequest request);
        Task<TransactionResponseModel> VerifyPayment(string transactionRef);
        Task<TransferRecipientModel> CreateTransferRecipient(TransferRequestDto request);
        Task<TransferRecipientListModel> GetListOfTransferRecipients();
        Task<TransferRecipientModel> CreateBeneficiary(BeneficiaryRequestDto request);
        Task<CustomerCreationResponse> CreateCustomer(CreateWalletRequest request);
        Task<PaystackGenericResponseDto<CreateVirtualAccountResponse>> CreateVirtualAccount(CreateVirtualAccountRequest requestDto);
        Task<PaystackGenericResponseDto<TransferResponseDto>> InitiateTransfer(TransferDataRequestDto requestDto);
        Task<PaystackGenericResponseDto<VerifyAccountResponseDto>> VerifyAccountNumber(VerifyAccountRequestDto requestDto);
        Task<ResponseDto<List<BankResponseDto>>> GetAllBanksAsync();
        Task<bool> VerifyTransfer(string reference);
    }
}
