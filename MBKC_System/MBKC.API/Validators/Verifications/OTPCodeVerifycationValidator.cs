using FluentValidation;
using MBKC.Service.DTOs.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.API.validators.Verifications
{
    public class OTPCodeVerifycationValidator: AbstractValidator<OTPCodeVerificationRequest>
    {
        public OTPCodeVerifycationValidator()
        {
            RuleFor(ocv => ocv.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email format.");

            RuleFor(ocv => ocv.OTPCode)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Matches(@"[0-9]{6,6}").WithMessage("{PropertyName} is required 6 digits.");
        }
    }
}
