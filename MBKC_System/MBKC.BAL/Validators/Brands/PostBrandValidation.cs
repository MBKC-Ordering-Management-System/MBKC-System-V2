using FluentValidation;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MBKC.BAL.Validators
{
    public class PostBrandValidation : AbstractValidator<PostBrandRequest>
    {
        private const int MAX_BYTES = 2048000;
        public PostBrandValidation()
        {
            #region Name
            RuleFor(b => b.Name)
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .Length(3, 120).WithMessage("{PropertyName} from {MinLength} to {MaxLength} characters.");
            #endregion

            #region Address
            RuleFor(b => b.Address)
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .Length(3, 255).WithMessage("{PropertyName} from {MinLength} to {MaxLength} characters.");
            #endregion

            #region Logo
            RuleFor(b => b.Logo)
                   .ChildRules(pro => pro.RuleFor(img => img.Length).ExclusiveBetween(0, MAX_BYTES)
                   .WithMessage($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB"));
            RuleFor(p => p.Logo)
                   .ChildRules(pro => pro.RuleFor(img => img.FileName).Must(FileUtil.HaveSupportedFileType).WithMessage("Logo is required extension type .png, .jpg, .jpeg"));
            #endregion

            #region ManagerEmail
            RuleFor(b => b.ManagerEmail)
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .Length(5, 100).WithMessage("{PropertyName} from {MinLength} to {MaxLength} characters.")
                     .Must(email => !string.IsNullOrEmpty(email) && Regex.IsMatch(email, @"@gmail\.com$"))
                     .WithMessage("{PropertyName} must be format @gmail.com");
            #endregion
        }
    }
}
