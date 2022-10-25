using Payment.Core.DTOs;
using Payment.Core.DTOs.BeneficiaryDto;
using Payment.Core.DTOs.PaystackDtos.BankDtos;

namespace Payment.Core.Interfaces
{
    public interface IBankAccountService
    {
        Task<ResponseDto<string>> AddBeneficiaryAsync(BeneficiaryRequestDto request, string walletId);
        Task<ResponseDto<PaginationResult<IEnumerable<BeneficiaryListResponseDto>>>> GetAllBeneficiariesAsync(int pageNumber, string walletId);
        Task <ResponseDto<string>> DeleteBeneficiary(string accountNumber, string walletId);
        Task<ResponseDto<VerifyAccountResponseDto>> VerifyAccountNumber(VerifyAccountRequestDto request);
    }
}
