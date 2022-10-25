using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.DTOs.PaystackDtos.BankDtos
{
    public class VerifyAccountRequestDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} is required.")]
        [JsonProperty("account_number")]
        public string AccountNumber { get; set; } = null!;

      
        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} is required.")]
        [JsonProperty("bank_code")]
        public string BankCode { get; set; } = null!;
    }
}
