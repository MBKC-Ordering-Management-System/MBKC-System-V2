using FluentValidation;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.API.Validators.Stores
{
    public class UpdateStoreValidator : AbstractValidator<UpdateStoreRequest>
    {
        private const int MAX_BYTES = 5242880;
        public UpdateStoreValidator()
        {
            RuleFor(usr => usr.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less then or equal to 100 characters.");

            RuleFor(usr => usr.Status)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Must(StringUtil.CheckStoreStatusName).WithMessage("{PropertyName} is required 'Active' or 'InActive' Status.");

            RuleFor(usr => usr.Logo)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Custom((logo, context) =>
                {
                    if (logo != null && logo.Length < 0 || logo != null && logo.Length > MAX_BYTES)
                    {
                        context.AddFailure($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB.");
                    }
                    if (logo != null && FileUtil.HaveSupportedFileType(logo.FileName) == false)
                    {
                        context.AddFailure("Logo is required extension type .png, .jpg, .jpeg, .webp.");
                    }
                });

            RuleFor(usr => usr.StoreManagerEmail)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email format.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");
        }
    }
}
