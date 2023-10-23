 using FluentValidation;
using MBKC.Service.DTOs.PartnerProducts;

namespace MBKC.API.Validators.PartnerProducts
{
    public class UpdatePartnerProductValidator : AbstractValidator<UpdatePartnerProductRequest>
    {
        public UpdatePartnerProductValidator()
        {
            RuleFor(mp => mp.ProductCode)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(50).WithMessage("{PropertyName} is required less then or equal to 50 characters.");
        }
    }
}
