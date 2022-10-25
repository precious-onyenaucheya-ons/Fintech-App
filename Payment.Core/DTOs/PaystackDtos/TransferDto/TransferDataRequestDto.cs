using Newtonsoft.Json;

namespace Payment.Core.DTOs.PaystackDtos.TransferDto
{
    public class TransferDataRequestDto
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("recipient")]
        public string RecipientCode { get; set; } = null!;

        [JsonProperty("source")]
        public string Source { get; set; } = null!;
    }
}
