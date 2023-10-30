using FluentValidation;
using MBKC.Service.DTOs.Orders;

namespace MBKC.PrivateAPI.Validators.Orders
{
    public class PutOrderIdValidator: AbstractValidator<PutOrderIdRequest>
    {
        public PutOrderIdValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.");
        }
    }
}
