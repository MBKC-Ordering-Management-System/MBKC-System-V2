using FluentValidation;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MBKC.API.Validators
{
    public class PostBrandValidator : AbstractValidator<PostBrandRequest>
    {
        private const int MAX_BYTES = 2048000;
        public PostBrandValidator()
        {
            #region Name
            RuleFor(b => b.Name)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is null.")
                     .NotEmpty().WithMessage("{PropertyName} is empty.")
                     .MaximumLength(120).WithMessage("{PropertyName} is required less than or equal to 120 characters.");

            #endregion

            #region Address
            RuleFor(b => b.Address)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is null.")
                     .NotEmpty().WithMessage("{PropertyName} is empty.")
                     .MaximumLength(255).WithMessage("{PropertyName} is required less than or equal to 255 characters.");
            #endregion

            #region Logo
            RuleFor(b => b.Logo)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .NotNull().WithMessage("{PropertyName} is not null.")
                   .ChildRules(pro => pro.RuleFor(img => img.Length).ExclusiveBetween(0, MAX_BYTES)
                   .WithMessage($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB."));
            RuleFor(p => p.Logo)
                   .ChildRules(pro => pro.RuleFor(img => img.FileName).Must(FileUtil.HaveSupportedFileType).WithMessage("Logo is required extension type .png, .jpg, .jpeg, .webp."));
            #endregion

            #region ManagerEmail
            RuleFor(b => b.ManagerEmail)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotEmpty().WithMessage("{PropertyName} is empty.")
                     .NotNull().WithMessage("{PropertyName} is null.")
                     .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.")
                     .EmailAddress().WithMessage("{PropertyName} must be email format.");
            #endregion
        }
    }
}
