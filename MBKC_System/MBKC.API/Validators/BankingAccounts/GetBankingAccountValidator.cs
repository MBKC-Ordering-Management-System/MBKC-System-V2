using FluentValidation;
using MBKC.Service.DTOs.BankingAccounts;

namespace MBKC.API.Validators.BankingAccounts
{
    public class GetBankingAccountValidator : AbstractValidator<BankingAccountRequest>
    {
        public GetBankingAccountValidator()
        {
            RuleFor(x => x.Id)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .NotNull().WithMessage("{PropertyName} is not null.")
                   .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
