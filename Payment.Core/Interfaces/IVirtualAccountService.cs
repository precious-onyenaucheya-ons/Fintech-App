using Payment.Core.DTOs;
using Payment.Core.DTOs.PaystackDtos.VirtualAccountDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.Interfaces
{
    public interface IVirtualAccountService
    {
        Task CreateVirtualAccount(CreateVirtualAccountRequest requestDto);
    }
}
