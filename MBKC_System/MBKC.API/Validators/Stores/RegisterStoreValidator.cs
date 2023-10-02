using FluentValidation;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.API.Validators.Stores
{
    public class RegisterStoreValidator : AbstractValidator<RegisterStoreRequest>
    {
        private const int MAX_BYTES = 5242880;
        public RegisterStoreValidator()
        {
            RuleFor(csr => csr.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(80).WithMessage("{PropertyName} is required less than or equal to 80 characters.");

            RuleFor(csr => csr.Logo)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
            .ChildRules(csr =>
            {
                csr.RuleFor(csr => csr.Length)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .ExclusiveBetween(0, MAX_BYTES).WithMessage($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB.");

                csr.RuleFor(csr => csr.FileName)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .Must(FileUtil.HaveSupportedFileType).WithMessage("{PropertyName} is required extension type .png, .jpg, .jpeg, .webp.");
            });

            RuleFor(csr => csr.KitchenCenterId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

            RuleFor(csr => csr.BrandId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

            RuleFor(csr => csr.StoreManagerEmail)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email format.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");
        }
    }
}
