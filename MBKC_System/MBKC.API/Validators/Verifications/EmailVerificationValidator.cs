using FluentValidation;
using MBKC.Service.DTOs.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.API.validators.Verifications
{
    public class EmailVerificationValidator: AbstractValidator<EmailVerificationRequest>
    {
        public EmailVerificationValidator()
        {
            RuleFor(ev => ev.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email format.");
        }
    }
}
