using FluentValidation;
using MBKC.Service.DTOs.Brands;

namespace MBKC.API.Validators.Brands
{
    public class GetBrandValidator : AbstractValidator<BrandRequest>
    {
        public GetBrandValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
