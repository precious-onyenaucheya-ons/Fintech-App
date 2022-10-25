namespace Payment.Core.DTOs.BankDtos
{
    public class BeneficiaryBankDto
    {
        public string BankCode { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public int PaystackBankId { get; set; } 
    }
}
