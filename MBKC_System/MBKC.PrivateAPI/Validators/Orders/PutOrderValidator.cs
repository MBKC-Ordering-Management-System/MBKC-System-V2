using FluentValidation;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Orders;

namespace MBKC.PrivateAPI.Validators.Orders
{
    public class PutOrderValidator:AbstractValidator<PutOrderRequest>
    {
        public PutOrderValidator()
        {
            RuleFor(x => x.Status)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{propertyName} is not empty.")
                .IsEnumName(typeof(OrderEnum.Status), caseSensitive: false).WithMessage("{PropertyName} is required some statuses such as: Upcoming or Preparing.");
        }
    }
}
