using FluentValidation;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Validators
{
    public class UpdateBrandValidation : AbstractValidator<UpdateBrandRequest>
    {
        private const int MAX_BYTES = 2048000;
        public UpdateBrandValidation()
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
                   .NotNull().WithMessage("{PropertyName} is null.")
                   .ChildRules(pro => pro.RuleFor(img => img.Length).ExclusiveBetween(0, MAX_BYTES)
                   .WithMessage($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB"));
            RuleFor(p => p.Logo)
                   .ChildRules(pro => pro.RuleFor(img => img.FileName).Must(FileUtil.HaveSupportedFileType).WithMessage("Logo is required extension type .png, .jpg, .jpeg, .webp"));
            #endregion

            #region Status
            RuleFor(b => b.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckBrandStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
            #endregion
        }
    }
}
