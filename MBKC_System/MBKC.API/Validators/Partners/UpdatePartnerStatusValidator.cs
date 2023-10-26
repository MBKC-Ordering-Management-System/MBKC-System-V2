using FluentValidation;
using MBKC.Service.DTOs.Partners;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Partners
{
    public class UpdatePartnerStatusValidator: AbstractValidator<UpdatePartnerStatusRequest>
    {
        public UpdatePartnerStatusValidator()
        {
            RuleFor(b => b.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckPartnerStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
        }
    }
}
