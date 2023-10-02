using FluentValidation;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.API.Validators.Brands
{
    public class UpdateBrandProfileValidator : AbstractValidator<UpdateBrandProfileRequest>
    {
        private const int MAX_BYTES = 2048000;
        public UpdateBrandProfileValidator()
        {
            RuleFor(ubpr => ubpr.Name)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is null.")
                     .NotEmpty().WithMessage("{PropertyName} is empty.")
                     .MaximumLength(120).WithMessage("{PropertyName} is required less than or equal to 120 characters.");


            RuleFor(ubpr => ubpr.Address)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is null.")
                     .NotEmpty().WithMessage("{PropertyName} is empty.")
                     .MaximumLength(255).WithMessage("{PropertyName} is required less than or equal to 255 characters.");

            RuleFor(ubpr => ubpr.Logo)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .ChildRules(pro => pro.RuleFor(img => img.Length).ExclusiveBetween(0, MAX_BYTES)
                   .WithMessage($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB"));
            RuleFor(ubpr => ubpr.Logo)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .ChildRules(pro => pro.RuleFor(img => img.FileName).Must(FileUtil.HaveSupportedFileType).WithMessage("Logo is required extension type .png, .jpg, .jpeg, .webp"));
        }
    }
}
