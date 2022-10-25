using Payment.Core.DTOs;
using Payment.Core.Interfaces;
using Payment.Core.Utilities.PaymentGatewaySettings;
using Payment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.Services
{
    public class BankService : IBankService
    {
        public readonly IUnitOfWork _unitOfWork;

        private readonly IPayStackPaymentHandler _payStackPaymentHandler;
        public BankService(IUnitOfWork unitOfWork, IPayStackPaymentHandler payStackPaymentHandler)
        {
            _unitOfWork = unitOfWork;
            _payStackPaymentHandler = payStackPaymentHandler;
        }
        public async Task<Bank> GetBankByIdAsync(string Id)
        {
            return await _unitOfWork.Banks.GetBankByIdAsync(Id);
        }
        public async Task<ResponseDto<List<BankResponseDto>>> GetAllBanksAsync()
        {
            return await _payStackPaymentHandler.GetAllBanksAsync();
        }
    }
}
