using Payment.Core.DTOs;
using Payment.Core.DTOs.PaystackDtos.FundAccountDto;
using Payment.Core.DTOs.TransactionsDto;
using Payment.Domain.Models;
using Paystack.Net.SDK.Models;


namespace Payment.Core.Interfaces
{
    public interface ITransactionService
    {
        Task<ResponseDto<PaginationResult<IEnumerable<TransactionHistoryResponseDto>>>>
            GetTransactionHistoryAsync(string walletId, int pageNumber);
        Task<ResponseDto<string>> Transfer(TransferRequestDto request, string walletId);
        Task<bool> VerifyTransferAsync(string transferReference);
      
    }
}
