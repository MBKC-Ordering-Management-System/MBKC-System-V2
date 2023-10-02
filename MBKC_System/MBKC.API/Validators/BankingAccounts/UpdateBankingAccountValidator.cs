using FluentValidation;
using MBKC.Service.DTOs.BankingAccounts;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.BankingAccounts
{
    public class UpdateBankingAccountValidator: AbstractValidator<UpdateBankingAccountRequest>
    {
        private const int MAX_BYTES = 2048000;
        public UpdateBankingAccountValidator()
        {
            RuleFor(ubar => ubar.BankName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");


            RuleFor(ubar => ubar.BankLogo)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .ChildRules(pro => pro.RuleFor(img => img.Length).ExclusiveBetween(0, MAX_BYTES).WithMessage($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB."));
            RuleFor(p => p.BankLogo)
                   .ChildRules(pro => pro.RuleFor(img => img.FileName).Must(FileUtil.HaveSupportedFileType).WithMessage("Logo is required extension type .png, .jpg, .jpeg, .webp."));

            RuleFor(ubar => ubar.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckBankingAccountStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
        }
    }
}
