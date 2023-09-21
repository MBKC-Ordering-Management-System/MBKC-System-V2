using FluentValidation;
using MBKC.BAL.DTOs.AccountTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Validators.Authentications
{
    public class AccountTokenRequestValidator: AbstractValidator<AccountTokenRequest>
    {
        public AccountTokenRequestValidator()
        {
            RuleFor(at => at.AccessToken)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.");

            RuleFor(at => at.RefreshToken)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.");
        }
    }
}
