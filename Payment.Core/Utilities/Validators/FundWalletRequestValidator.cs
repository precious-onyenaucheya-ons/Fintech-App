using FluentValidation;
using Payment.Core.DTOs.PaystackDtos.FundAccountDto;

namespace Payment.Core.Utilities.Validators
{
    public class FundWalletRequestValidator : AbstractValidator<FundWalletRequest>
    {
        public FundWalletRequestValidator()
        {
            RuleFor(w => w.Email).EmailAddress();
            RuleFor(w => w.Amount).GreaterThan(0)
                .NotEmpty()
                .NotNull().WithMessage("Invalid amount format");
        }
    }
}
