using FluentValidation;
using MBKC.Service.DTOs.MoneyExchanges;

namespace MBKC.API.Validators.MoneyExchanges
{
    public class GetMoneyExchangeValidator : AbstractValidator<MoneyExchangeRequest>
    {
        public GetMoneyExchangeValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
