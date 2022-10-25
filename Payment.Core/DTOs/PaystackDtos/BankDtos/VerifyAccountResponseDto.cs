using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.DTOs.PaystackDtos.BankDtos
{
    public class VerifyAccountResponseDto
    {
        [JsonProperty("account_number")]
        public string AccountNumber { get; set; } = null!;
        [JsonProperty("account_name")]
        public string AccountName { get; set; } = null!;

    }
}
