using Newtonsoft.Json;
using Payment.Core.DTOs.PaystackDtos.BankDtos;

namespace Payment.Core.DTOs.PaystackDtos.VirtualAccountDtos
{
    public class CreateVirtualAccountResponse
    {
        [JsonProperty("account_number")]
        public string AccountNumber { get; set; } = null!;
        [JsonProperty("account_name")]
        public string AccountName { get; set; } = null!;
        [JsonProperty("bank")]
        public CreateBankResponse Bank { get; set; } = null!;
    }
}
