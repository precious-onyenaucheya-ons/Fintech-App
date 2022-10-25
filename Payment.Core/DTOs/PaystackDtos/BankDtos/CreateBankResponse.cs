using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.DTOs.PaystackDtos.BankDtos
{
    public class CreateBankResponse
    {
        [JsonProperty("name")]
        public string BankName { get; set; } = null!;
        [JsonProperty("id")]
        public int PaystackBankId { get; set; }
    }
}
