using FluentValidation;
using MBKC.BAL.DTOs.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Validators.Accounts
{
    public class ResetPasswordRequestValidator: AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(ev => ev.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email format.");

            RuleFor(ev => ev.NewPassword)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(50).WithMessage("{PropertyName} is required less than or equal to 50 characters.");
        }
    }
}
