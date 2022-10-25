using Newtonsoft.Json;

namespace Payment.Core.DTOs.TransactionsDto
{
    public class TransferResponseDto
    {
        [JsonProperty(PropertyName = "transfer_code")]
        public string TransferCode { get; set; } = null!;

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } = null!;

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; } = null!;

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; } = null!;

        [JsonProperty(PropertyName = "reason")]
        public string Narration { get; set; } = null!;

        [JsonProperty(PropertyName = "reference")]
        public string reference { get; set; }= null!;

        [JsonProperty(PropertyName = "createdAt")]
        public string createdAt { get; set; } = null!;

        [JsonProperty(PropertyName = "updatedAt")]
        public string updatedAt { get; set; } = null!;

        public string TransactionType { get; set; } = "withdrawal";
    }
}
