using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.DTOs.WalletDtos
{
    public class CreateWalletResponse
    {
        public string Id { get; set; } = null!;
        public string PaystackCustomerCode { get; set; } = null!;
    }
}
