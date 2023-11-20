using FluentValidation;
using MBKC.Service.DTOs.UserDevices;

namespace MBKC.API.Validators.UserDevices
{
    public class CreateUserDeviceValidator: AbstractValidator<CreateUserDeviceRequest>
    {
        public CreateUserDeviceValidator()
        {
            RuleFor(x => x.FCMToken)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.");
        }
    }
}
