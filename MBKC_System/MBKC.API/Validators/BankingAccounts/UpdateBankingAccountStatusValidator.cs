using FluentValidation;
using MBKC.Service.DTOs.BankingAccounts;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.BankingAccounts
{
    public class UpdateBankingAccountStatusValidator: AbstractValidator<UpdateBankingAccountStatusRequest>
    {
        public UpdateBankingAccountStatusValidator()
        {
            RuleFor(ubasr => ubasr.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckBankingAccountStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
        }
    }
}
