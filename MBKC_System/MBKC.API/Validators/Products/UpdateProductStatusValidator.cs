using FluentValidation;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Products;

namespace MBKC.API.Validators.Products
{
    public class UpdateProductStatusValidator: AbstractValidator<UpdateProductStatusRequest>
    {
        public UpdateProductStatusValidator()
        {
            RuleFor(x => x.Status)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty()
                .IsEnumName(typeof(ProductEnum.Status), caseSensitive: false).WithMessage("{PropertyName} is required some statuses such as: INACTIVE, ACTIVE.")
                .NotEqual(ProductEnum.Status.DEACTIVE.ToString().ToLower()).WithMessage("{PropertyName} is required some statuses such as: INACTIVE, ACTIVE.");
        }
    }
}
