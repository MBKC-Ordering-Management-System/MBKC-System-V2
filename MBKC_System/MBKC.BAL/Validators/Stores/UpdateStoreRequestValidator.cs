using FluentValidation;
using MBKC.BAL.DTOs.Stores;
using MBKC.BAL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Validators.Stores
{
    public class UpdateStoreRequestValidator : AbstractValidator<UpdateStoreRequest>
    {
        private const int MAX_BYTES = 5242880;
        public UpdateStoreRequestValidator()
        {
            RuleFor(ckcr => ckcr.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less then or equal to 100 characters.");

            RuleFor(ckcr => ckcr.Status)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Must(StringUtil.CheckStoreStatusName).WithMessage("{PropertyName} is required \"Active\" or \"InActive\" Status");

            RuleFor(ukcr => ukcr.NewLogo)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Custom((newLogo, context) =>
                {
                    if (newLogo != null && newLogo.Length < 0 || newLogo.Length > MAX_BYTES)
                    {
                        context.AddFailure($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB.");
                    }
                    if (newLogo != null && FileUtil.HaveSupportedFileType(newLogo.FileName) == false)
                    {
                        context.AddFailure("Logo is required extension type .png, .jpg, .jpeg, .webp.");
                    }
                });

            RuleFor(ukcr => ukcr.DeletedLogo)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Custom((deletedLogo, context) =>
                {
                    if (deletedLogo != null && StringUtil.CheckUrlString(deletedLogo) == false)
                    {
                        context.AddFailure("Deleted Logo Url is invalid URL format.");
                    }
                });

            RuleFor(ckcr => ckcr.StoreManagerEmail)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email format.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");
        }
    }
}
