using Payment.Core.DTOs;
using Payment.Core.Utilities.PaymentGatewaySettings;
using Payment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.Interfaces
{
    public interface IBankService
    {
        Task<ResponseDto<List<BankResponseDto>>> GetAllBanksAsync();
        Task<Bank> GetBankByIdAsync(string Id);
    }
}
