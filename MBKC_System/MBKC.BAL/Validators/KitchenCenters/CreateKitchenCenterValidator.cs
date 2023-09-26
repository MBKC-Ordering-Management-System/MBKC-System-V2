using FluentValidation;
using MBKC.BAL.DTOs.KitchenCenters;
using MBKC.BAL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Validators.KitchenCenters
{
    public class CreateKitchenCenterValidator: AbstractValidator<CreateKitchenCenterRequest>
    {
        private const int MAX_BYTES = 5242880;
        public CreateKitchenCenterValidator()
        {
            RuleFor(ckcr => ckcr.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less then or equal to 100 characters.");

            RuleFor(ckcr => ckcr.Address)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(255).WithMessage("{PropertyName} is required less then or equal to 255 characters.");

            RuleFor(ckcr => ckcr.Logo)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.");

            RuleFor(ckcr => ckcr.Logo.Length)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ExclusiveBetween(0, MAX_BYTES).WithMessage($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB.");

            RuleFor(ckcr => ckcr.Logo.FileName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(FileUtil.HaveSupportedFileType).WithMessage("{PropertyName} is required extension type .png, .jpg, .jpeg, .webp.");

            RuleFor(ckcr => ckcr.ManagerEmail)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email format.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");
        }
    }
}
