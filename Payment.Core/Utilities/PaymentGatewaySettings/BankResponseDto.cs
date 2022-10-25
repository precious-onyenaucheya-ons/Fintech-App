using Newtonsoft.Json;

namespace Payment.Core.Utilities.PaymentGatewaySettings
{
    public class BankResponseDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}