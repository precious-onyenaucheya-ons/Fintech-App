using Payment.Domain.Common;

namespace Payment.Domain.Models
{
    public class BankAccount : BaseEntity
    {
        public string AccountNumber { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public string BankId { get; set; } = null!;
        public string RecipientCode { get; set; } = null!;
        public string WalletId { get; set; } = null!;
        public bool IsInternal { get; set; }

        public Bank Bank { get; set; } = null!;
        public Wallet? Wallet { get; set; }
    }
}
