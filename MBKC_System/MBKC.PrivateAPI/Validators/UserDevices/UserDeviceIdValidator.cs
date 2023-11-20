using FluentValidation;
using MBKC.Service.DTOs.UserDevices;

namespace MBKC.PrivateAPI.Validators.UserDevices
{
    public class UserDeviceIdValidator: AbstractValidator<UserDeviceIdRequest>
    {
        public UserDeviceIdValidator()
        {
            RuleFor(x => x.UserDeviceId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .GreaterThan(0).WithMessage("{PropertyName} is not subtable id in the system.");
        }
    }
}
