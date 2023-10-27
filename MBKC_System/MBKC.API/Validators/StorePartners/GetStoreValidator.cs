using FluentValidation;
using MBKC.Service.DTOs.StorePartners;

namespace MBKC.API.Validators.StorePartners
{
    public class GetStoreValidator : AbstractValidator<StoreRequest>
    {
        public GetStoreValidator()
        {
            RuleFor(x => x.StoreId)
               .Cascade(CascadeMode.StopOnFirstFailure)
               .NotNull().WithMessage("{PropertyName} is not null.")
               .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
