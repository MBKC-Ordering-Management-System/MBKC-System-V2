using FluentValidation;
using MBKC.Service.DTOs.PartnerProducts;
using MBKC.Service.Utils;

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

            RuleFor(mp => mp.Price)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than or equal to 0.");

            RuleFor(mp => mp.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckPartnerProductStatusName).WithMessage("{PropertyName} is required some types such as: Available, Out_Of_Stock_Today, or Out_Of_Stock_Indentifinitely.");
        }
    }
}
