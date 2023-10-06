using FluentValidation;
using MBKC.Service.DTOs.Accounts;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Accounts
{
    public class UpdateAccountValidator: AbstractValidator<UpdateAccountRequest>
    {
        public UpdateAccountValidator()
        {
            RuleFor(ev => ev.NewPassword)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Must(StringUtil.IsMD5).WithMessage("New password must convert with MD5 althorigm before saving in the system.");
        }
    }
}
