using Newtonsoft.Json;

namespace Payment.Core.DTOs.PaystackDtos.VirtualAccountDtos
{
    public class CreateVirtualAccountRequest
    {
        [JsonProperty("customer")]
        public string PaystackCustomerCode { get; set; } = null!;
        public string WalletId { get; set; } = null!;
        [JsonProperty("preferred_bank")]
        public string PreferredBank { get; set; } = "test-bank";
    }
}
