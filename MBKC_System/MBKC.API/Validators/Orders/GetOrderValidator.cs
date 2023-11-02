using FluentValidation;
using MBKC.Service.DTOs.Orders;

namespace MBKC.API.Validators.Orders
{
    public class GetOrderValidator  : AbstractValidator<OrderRequest>
    {
        public GetOrderValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
