using AutoMapper;
using Payment.Core.DTOs.PaystackDtos.VirtualAccountDtos;
using Payment.Core.Interfaces;
using Payment.Domain.Models;
using Serilog;
using System.Transactions;

namespace Payment.Core.Services
{
    public class VirtualAccountService : IVirtualAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IPayStackPaymentHandler _payStackPaymentHandler;

        public VirtualAccountService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger, IPayStackPaymentHandler payStackPaymentHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _payStackPaymentHandler = payStackPaymentHandler;
        }

        public async Task CreateVirtualAccount(CreateVirtualAccountRequest requestDto)
        {
            Bank bank;
            VirtualAccount virtualAccount;

            using var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var response = await _payStackPaymentHandler.CreateVirtualAccount(requestDto);
                if (!response.Status)
                {
                    _logger.Information($"Could not create virtual account for wallet " +
                        $"{requestDto.WalletId} : {response.Message}");
                    throw new Exception();
                }

                // create bank if it doesn't exist
                bank = await _unitOfWork.Banks.GetBankByPaystackIdAsync(response.Data.Bank.PaystackBankId);
                if (bank == null)
                {
                    bank = _mapper.Map<Bank>(response.Data.Bank);
                    await _unitOfWork.Banks.AddAsync(bank);
                }

                // create virtual account
                virtualAccount = _mapper.Map<VirtualAccount>(response.Data);
                virtualAccount.BankId = bank.Id;
                virtualAccount.WalletId = requestDto.WalletId;
                await _unitOfWork.VirtualAccounts.AddAsync(virtualAccount);
            }
            catch (Exception ex)
            {
                _logger.Error($"Could not create wallet: {ex.Message}");
                scope.Dispose();
                throw;
            }
        }
    }
}
