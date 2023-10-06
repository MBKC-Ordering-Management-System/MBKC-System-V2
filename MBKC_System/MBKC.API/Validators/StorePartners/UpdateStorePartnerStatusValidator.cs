using FluentValidation;
using MBKC.Service.DTOs.StorePartners;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.StorePartners
{
    public class UpdateStorePartnerStatusValidator : AbstractValidator<UpdateStorePartnerStatusRequest>
    {
        public UpdateStorePartnerStatusValidator()
        {
            #region Status
            RuleFor(s => s.Status)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull().WithMessage("{PropertyName} is not null.")
                    .NotEmpty().WithMessage("{PropertyName} is not empty.")
                    .Must(StringUtil.CheckBrandStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
            #endregion
        }
    }
}
