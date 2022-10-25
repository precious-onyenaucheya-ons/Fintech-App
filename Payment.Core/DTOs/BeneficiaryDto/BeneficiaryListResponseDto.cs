using Payment.Core.DTOs.BankDtos;
using Payment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.DTOs.BeneficiaryDto
{
    public class BeneficiaryListResponseDto
    {
        public string BankAccountId { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public string AccountName{ get; set; } = null!;
        public string BankId { get; set; } = null!;
        public string RecipientCode { get; set; } = null!;
        public string WalletId { get; set; } = null!;
        public BeneficiaryBankDto bank { get; set; } = null!;
    }
}
