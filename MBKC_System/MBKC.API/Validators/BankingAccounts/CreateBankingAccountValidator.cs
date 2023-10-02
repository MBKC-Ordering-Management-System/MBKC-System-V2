using FluentValidation;
using MBKC.Service.DTOs.BankingAccounts;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.BankingAccounts
{
    public class CreateBankingAccountValidator: AbstractValidator<CreateBankingAccountRequest>
    {
        private const int MAX_BYTES = 2048000;
        public CreateBankingAccountValidator()
        {
            RuleFor(cbar => cbar.BankName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");

            RuleFor(cbar => cbar.NumberAccount)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Must(StringUtil.IsDigitString).WithMessage("{PropertyName} is required that contains only digit.")
                .Length(10, 20).WithMessage("{PropertyName} is required from 10 to 20 digits.");

            
            RuleFor(cbar => cbar.BankLogo)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .NotNull().WithMessage("{PropertyName} is not null.")
                   .ChildRules(pro => pro.RuleFor(img => img.Length).ExclusiveBetween(0, MAX_BYTES).WithMessage($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB."));
            RuleFor(p => p.BankLogo)
                   .ChildRules(pro => pro.RuleFor(img => img.FileName).Must(FileUtil.HaveSupportedFileType).WithMessage("Logo is required extension type .png, .jpg, .jpeg, .webp."));
            
        }
    }
}
