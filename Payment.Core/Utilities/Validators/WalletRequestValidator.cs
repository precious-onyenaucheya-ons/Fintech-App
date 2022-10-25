using FluentValidation;
using Payment.Core.DTOs.WalletDtos;

namespace Payment.Core.Utilities.Validators
{
    public class WalletRequestValidator : AbstractValidator<CreateWalletRequest>
    {
        public WalletRequestValidator()
        {
            RuleFor(w => w.FirstName).HumanName();
            RuleFor(w => w.LastName).HumanName();
            RuleFor(w => w.UserEmail).EmailAddress();
            RuleFor(w => w.Pin).Matches(@"^[0-9]{4}$");
            RuleFor(w => w.UserId).IdGuidString();
        }

    }
}
