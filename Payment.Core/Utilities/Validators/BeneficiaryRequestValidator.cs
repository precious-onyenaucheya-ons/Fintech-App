using FluentValidation;
using Payment.Core.DTOs.BeneficiaryDto;

namespace Payment.Core.Utilities.Validators
{
    public class BeneficiaryRequestValidator: AbstractValidator<BeneficiaryRequestDto>
    {
        public BeneficiaryRequestValidator()
        {
            RuleFor(x => x.AccountName).NotEmpty().NotNull().WithMessage("*Required");
            RuleFor(x => x.RecipientType).NotEmpty().NotNull().WithMessage("*Required");
            RuleFor(x => x.AccountNumber).NotEmpty().NotNull().WithMessage("*Required");
            RuleFor(x => x.BeneficiaryBank).NotEmpty().NotNull().WithMessage("*Required");
        }
    }
}
