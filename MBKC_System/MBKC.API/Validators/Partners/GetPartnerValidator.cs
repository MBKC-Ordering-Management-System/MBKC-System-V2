using FluentValidation;
using MBKC.Service.DTOs.Partners;

namespace MBKC.API.Validators.Partners
{
    public class GetPartnerValidator : AbstractValidator<PartnerRequest>
    {
        public GetPartnerValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
