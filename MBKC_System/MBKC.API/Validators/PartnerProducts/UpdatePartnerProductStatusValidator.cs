using FluentValidation;
using MBKC.Service.Utils;
using MBKC.Service.DTOs.PartnerProducts;

namespace MBKC.API.Validators.PartnerProducts
{
    public class UpdatePartnerProductStatusValidator : AbstractValidator<UpdatePartnerProductStatusRequest>
    {
        public UpdatePartnerProductStatusValidator()
        {
            RuleFor(updatePartnerProductStatusRequest => updatePartnerProductStatusRequest.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckPartnerStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
        }
    }
}
