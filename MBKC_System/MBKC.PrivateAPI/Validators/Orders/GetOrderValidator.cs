using FluentValidation;
using MBKC.Service.DTOs.Orders;

namespace MBKC.PrivateAPI.Validators.Orders
{
    public class GetOrderValidator: AbstractValidator<GetOrderRequest>
    {
        public GetOrderValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.");
        }
    }
}
