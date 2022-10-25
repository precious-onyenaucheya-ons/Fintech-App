using FluentValidation;
using Payment.Core.DTOs.TransactionsDto;

namespace Payment.Core.Utilities.Validators
{
    public class TransferRequestValidator: AbstractValidator<TransferRequestDto>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.AccountName).NotEmpty().NotNull().WithMessage("*Required");
            RuleFor(x => x.RecipientType).NotEmpty().NotNull().WithMessage("*Required");
            RuleFor(x => x.Amount).NotEmpty().NotNull().WithMessage("*Required");
            RuleFor(x => x.AccountNumber).NotEmpty().NotNull().WithMessage("*Required");
            RuleFor(x => x.Description).NotEmpty().NotNull().WithMessage("*Required");
            RuleFor(x => x.TransferPin).NotEmpty().Matches(@"^[0-9]{4}$").WithMessage("non numerical input");
        }
    }
}
