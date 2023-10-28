using FluentValidation;
using MBKC.Service.DTOs.StorePartners;

namespace MBKC.API.Validators.StorePartners
{
    public class GetStorePartnerValidator : AbstractValidator<StorePartnerRequest>
    {
        public GetStorePartnerValidator()
        {
            RuleFor(x => x.StoreId)
               .Cascade(CascadeMode.StopOnFirstFailure)
               .NotNull().WithMessage("{PropertyName} is not null.")
               .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

            RuleFor(x => x.PartnerId)
               .Cascade(CascadeMode.StopOnFirstFailure)
               .NotNull().WithMessage("{PropertyName} is not null.")
               .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
