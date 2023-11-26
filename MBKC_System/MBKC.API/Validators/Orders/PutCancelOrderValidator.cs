using FluentValidation;
using MBKC.Service.DTOs.Orders;

namespace MBKC.API.Validators.Orders
{
    public class PutCancelOrderValidator:AbstractValidator<PutCancelOrderRequest>
    {
        public PutCancelOrderValidator()
        {
            RuleFor(x => x.RejectedReason)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(200).WithMessage("{PropertyName} is required less than or equal to 200 characters.");
        }
    }
}
