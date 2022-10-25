using Payment.Core.DTOs.BankDtos;

namespace Payment.Core.DTOs.VirtualAccountDtos
{
    public class VirtualAccountDto
    {
        public string AccountNumber { get; set; } = null!;
        public string WalletId { get; set; } = null!;
        public BankDto? Bank { get; set; }
    }
}
