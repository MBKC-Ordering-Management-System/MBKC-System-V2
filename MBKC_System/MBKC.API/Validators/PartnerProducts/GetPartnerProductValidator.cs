using FluentValidation;
using MBKC.Service.DTOs.PartnerProducts;

namespace MBKC.API.Validators.PartnerProducts
{
    public class GetPartnerProductValidator : AbstractValidator<PartnerProductRequest>
    {
        public GetPartnerProductValidator()
        {
            RuleFor(x => x.ProductId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

            RuleFor(x => x.PartnertId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

            RuleFor(x => x.StoreId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
