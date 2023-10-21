using FluentValidation;
using MBKC.Service.Utils;
using MBKC.Service.DTOs.StorePartners;

namespace MBKC.API.Validators.StorePartners
{
    public class UpdateStorePartnerValidator : AbstractValidator<UpdateStorePartnerRequest>
    {
        public UpdateStorePartnerValidator()
        {
            #region UserName
            RuleFor(s => s.UserName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less then or equal to 100 characters.");
            #endregion

            #region Password
            RuleFor(s => s.Password)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(50).WithMessage("{PropertyName} is required less then or equal to 50 characters.");
            #endregion

            #region Status
            RuleFor(s => s.Status)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull().WithMessage("{PropertyName} is not null.")
                    .NotEmpty().WithMessage("{PropertyName} is not empty.")
                    .Must(StringUtil.CheckBrandStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
            #endregion

            #region Commission
            RuleFor(updateStorePartnerRequest => updateStorePartnerRequest.Commission)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .InclusiveBetween(0, 100).WithMessage("{PropertyName} must be between 0 and 100.");
            #endregion
        }
    }
}
