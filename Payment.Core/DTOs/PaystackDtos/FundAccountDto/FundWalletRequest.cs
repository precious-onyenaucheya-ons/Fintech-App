using System.ComponentModel.DataAnnotations;

namespace Payment.Core.DTOs.PaystackDtos.FundAccountDto
{
    public class FundWalletRequest
    {
        [Required]
        public int Amount { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
