using FluentValidation;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Stores
{
    public class ConfirmStoreRegistrationValidator : AbstractValidator<ConfirmStoreRegistrationRequest>
    {
        public ConfirmStoreRegistrationValidator()
        {
            RuleFor(csrr => csrr.Status)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Must(StringUtil.CheckConfirmStoreRegistrationStatusName).WithMessage("{PropertyName} is required ACTIVE or REJECTED.");

            RuleFor(csrr => csrr.RejectedReason)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(x => x.RuleFor(csrr => csrr)
                                             .Cascade(CascadeMode.StopOnFirstFailure)
                                             .MaximumLength(250).WithMessage("{PropertyName} is required less than or equal to 250 characters."));
        }
    }
}
