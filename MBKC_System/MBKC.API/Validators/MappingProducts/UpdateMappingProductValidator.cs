 using FluentValidation;
using MBKC.Service.DTOs.MappingProducts;

namespace MBKC.API.Validators.MappingProducts
{
    public class UpdateMappingProductValidator : AbstractValidator<UpdateMappingProductRequest>
    {
        public UpdateMappingProductValidator()
        {
            RuleFor(mp => mp.ProductCode)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(50).WithMessage("{PropertyName} is required less then or equal to 50 characters.");
        }
    }
}
