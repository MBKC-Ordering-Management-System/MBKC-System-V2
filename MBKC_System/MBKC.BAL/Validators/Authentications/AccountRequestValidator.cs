using FluentValidation;
using MBKC.BAL.DTOs.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Validators.Authentications
{
    public class AccountRequestValidator: AbstractValidator<AccountRequest>
    {
        public AccountRequestValidator()
        {
            RuleFor(ar => ar.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email Format.");

            RuleFor(ar => ar.Password)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.");
        }
    }
}
