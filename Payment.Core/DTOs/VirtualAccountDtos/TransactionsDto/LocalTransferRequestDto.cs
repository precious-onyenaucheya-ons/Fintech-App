using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.DTOs.TransactionsDto
{
    public class LocalTransferRequestDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public string DestinationWalletCode { get; set; } = null!;
        public string TransactionPIN { get; set; } = null!;
    }
}