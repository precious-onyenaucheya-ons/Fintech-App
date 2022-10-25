using AutoMapper;
using Payment.Core.DTOs;
using Payment.Core.DTOs.BankDtos;
using Payment.Core.DTOs.BeneficiaryDto;
using Payment.Core.DTOs.PaystackDtos.BankDtos;
using Payment.Core.DTOs.PaystackDtos.VirtualAccountDtos;
using Payment.Core.DTOs.TransactionsDto;
using Payment.Core.DTOs.VirtualAccountDtos;
using Payment.Core.DTOs.WalletDtos;
using Payment.Domain.Models;

namespace Payment.Core.Utilities.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<TransferResponseDto, Transaction>().ReverseMap();
            CreateMap<CreateWalletRequest, Wallet>();
            CreateMap<Wallet, CreateWalletResponse>();
            CreateMap<TransferRequestDto, BeneficiaryRequestDto>();
            CreateMap<Bank, BeneficiaryBankDto>().ReverseMap();
            CreateMap<CreateBankResponse, Bank>();
            CreateMap<CreateVirtualAccountResponse, VirtualAccount>();
            CreateMap<Transaction, TransactionHistoryResponseDto>()
                .ForMember(t => t.TransactionId, th => th.MapFrom(src => src.Id));

            CreateMap<BeneficiaryListResponseDto, BankAccount>()
              .ForMember(t => t.Id, th => th.MapFrom(src => src.BankAccountId))
              .ReverseMap();
            CreateMap<Bank, BankDto>();
            CreateMap<VirtualAccount, VirtualAccountDto>()
                .ForMember(dest => dest.Bank,
                opt => opt.MapFrom(src => src.Bank));
            CreateMap<Wallet, WalletResponseDto>()
                .ForMember(dest => dest.VirtualAccount,
                opt => opt.MapFrom(src => src.VirtualAccount));
        }
    }
}
