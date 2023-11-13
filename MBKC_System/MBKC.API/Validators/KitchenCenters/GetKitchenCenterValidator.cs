using FluentValidation;
using MBKC.Service.DTOs.KitchenCenters;

namespace MBKC.API.Validators.KitchenCenters
{
    public class GetKitchenCenterValidator : AbstractValidator<KitchenCenterIdRequest>
    {
        public GetKitchenCenterValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
