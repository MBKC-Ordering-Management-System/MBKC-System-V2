using FluentValidation;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Cashiers.Requests;
using StackExchange.Redis;

namespace MBKC.API.Validators.Cashiers
{
    public class UpdateCashierStatusValidator: AbstractValidator<UpdateCashierStatusRequest>
    {
        public UpdateCashierStatusValidator()
        {
            RuleFor(cpr => cpr.Status)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .IsEnumName(typeof(ProductEnum.Status), caseSensitive: false).WithMessage("{PropertyName} is required some statuses such as: INACTIVE, ACTIVE.")
                .ChildRules(x => x.RuleFor(x => x.ToLower())
                                         .Cascade(CascadeMode.StopOnFirstFailure)
                                         .NotEqual(ProductEnum.Status.DISABLE.ToString().ToLower()).WithMessage("Status is required some statuses such as: INACTIVE, ACTIVE."));
        }
    }
}
