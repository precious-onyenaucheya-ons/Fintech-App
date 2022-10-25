using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.DTOs.WalletDtos
{
    public class CreateWalletRequest
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        public string? Name { get{ return $"{FirstName} {LastName}"; } }
        [JsonProperty("email")]
        public string UserEmail { get; set; }
        public string UserId { get; set; }
        public string Pin { get; set; }
    }
}
