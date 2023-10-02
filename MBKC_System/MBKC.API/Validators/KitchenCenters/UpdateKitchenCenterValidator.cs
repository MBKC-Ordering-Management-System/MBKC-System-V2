using FluentValidation;
using MBKC.Service.DTOs.KitchenCenters;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.API.Validators.KitchenCenters
{
    public class UpdateKitchenCenterValidator : AbstractValidator<UpdateKitchenCenterRequest>
    {
        private const int MAX_BYTES = 5242880;
        public UpdateKitchenCenterValidator()
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

            RuleFor(ckcr => ckcr.Status)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Must(StringUtil.CheckKitchenCenterStatusName).WithMessage("{PropertyName} is required 'Active' or 'InActive' Status.");

            RuleFor(ukcr => ukcr.Logo)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Custom((Logo, context) =>
                {
                    if (Logo != null && Logo.Length < 0 || Logo != null && Logo.Length > MAX_BYTES)
                    {
                        context.AddFailure($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB.");
                    }
                    if (Logo != null && FileUtil.HaveSupportedFileType(Logo.FileName) == false)
                    {
                        context.AddFailure("Logo is required extension type .png, .jpg, .jpeg, .webp.");
                    }
                });

            RuleFor(ckcr => ckcr.ManagerEmail)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email format.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");
        }
    }
}
