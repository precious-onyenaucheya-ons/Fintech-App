using FluentValidation;
using Payment.Core.DTOs.PaystackDtos.BankDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.Utilities.Validators
{
    public class VerifyAccountValidator: AbstractValidator<VerifyAccountRequestDto>
    {
        public VerifyAccountValidator()
        {
            RuleFor(x => x.AccountNumber).NotEmpty().Matches(@"^[0-9]{10}$").WithMessage("non numerical input");
        }
    }
}
