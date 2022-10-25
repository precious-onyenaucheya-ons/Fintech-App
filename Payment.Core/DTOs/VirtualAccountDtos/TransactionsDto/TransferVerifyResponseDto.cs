using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.DTOs.VirtualAccountDtos.TransactionsDto
{
    public class TransferVerifyResponseDto
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } = null!;
    }
}
