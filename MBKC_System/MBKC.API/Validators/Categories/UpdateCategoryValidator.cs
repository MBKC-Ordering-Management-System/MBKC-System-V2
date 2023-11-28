using FluentValidation;
using MBKC.Service.DTOs.Categories;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.API.Validators.Categories
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
    {
        private const int MAX_BYTES = 5242880;
        public UpdateCategoryValidator()
        {
            #region Name
            RuleFor(c => c.Name)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");
            #endregion

            #region Status
            RuleFor(c => c.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckCategoryStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
            #endregion

            #region Description
            RuleFor(c => c.Description)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");
            #endregion

            #region ImageUrl
            RuleFor(c => c.ImageUrl)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .ChildRules(category => category.RuleFor(img => img.Length).ExclusiveBetween(0, MAX_BYTES)
                   .WithMessage($"Image is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB"));
            RuleFor(c => c.ImageUrl)
                   .ChildRules(pro => pro.RuleFor(img => img.FileName).Must(FileUtil.HaveSupportedFileType).WithMessage("Image is required extension type .png, .jpg, .jpeg, .webp"));
            #endregion

            #region DisplayOrder
            RuleFor(c => c.DisplayOrder)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .GreaterThan(0)
                   .WithMessage("{PropertyName} must be greater than 0");
            #endregion
        }
    }
}
