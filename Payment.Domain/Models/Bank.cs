using Payment.Domain.Common;

namespace Payment.Domain.Models
{
    public class Bank : BaseEntity
    {
        public string? BankCode { get; set; }
        public string BankName { get; set; } = null!;
        public string? CountryCode { get; set; }
        public int? PaystackBankId { get; set; }

        public ICollection<VirtualAccount> VirtualAccounts{ get; set; } = null!;
        public ICollection<BankAccount> BankAccounts { get; set; } = null!;
    }
}
