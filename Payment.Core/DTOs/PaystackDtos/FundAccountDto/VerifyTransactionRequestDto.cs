using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.DTOs.PaystackDtos.FundAccountDto
{
    public class VerifyTransactionRequestDto
    {
        public string Reference { get; set; } = string.Empty;
    }
}
