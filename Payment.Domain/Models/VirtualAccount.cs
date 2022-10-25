using Payment.Domain.Common;
using Payment.Domain.Enums;

namespace Payment.Domain.Models
{
    public class VirtualAccount : BaseEntity
    {
        public string AccountName { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public string? AccountReference { get; set; }
        public VirtualAccountStatus Status { get; set; } = VirtualAccountStatus.Active;
        public string BankId { get; set; } = null!;
        public string WalletId { get; set; } = null!;

        public Wallet? Wallet { get; set; }
        public Bank? Bank { get; set; }
    }
}
