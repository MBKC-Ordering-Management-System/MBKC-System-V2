using FluentValidation;
using MBKC.Service.DTOs.Accounts;

namespace MBKC.API.Validators.Accounts
{
    public class AccountIdValidator: AbstractValidator<AccountIdRequest>
    {
        public AccountIdValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
