using FluentValidation;
using MBKC.Service.DTOs.PartnerProducts;

namespace MBKC.API.Validators.PartnerProducts
{
    public class CreatePartnerProductValidator : AbstractValidator<PostPartnerProductRequest>
    {
        public CreatePartnerProductValidator()
        {
            RuleFor(mp => mp.ProductCode)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(50).WithMessage("{PropertyName} is required less then or equal to 50 characters.");

            RuleFor(mp => mp.Price)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(mp => mp.PartnerId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(mp => mp.ProductId)
               .Cascade(CascadeMode.StopOnFirstFailure)
               .NotNull().WithMessage("{PropertyName} is not null.")
               .NotEmpty().WithMessage("{PropertyName} is not empty.")
               .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(mp => mp.StoreId)
             .Cascade(CascadeMode.StopOnFirstFailure)
             .NotNull().WithMessage("{PropertyName} is not null.")
             .NotEmpty().WithMessage("{PropertyName} is not empty.")
             .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
        }
    }
}
