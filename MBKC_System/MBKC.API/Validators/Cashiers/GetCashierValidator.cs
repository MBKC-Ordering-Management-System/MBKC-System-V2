using FluentValidation;
using MBKC.Service.DTOs.Cashiers.Requests;

namespace MBKC.API.Validators.Cashiers
{
    public class GetCashierValidator:AbstractValidator<CashierRequest>
    {
        public GetCashierValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
