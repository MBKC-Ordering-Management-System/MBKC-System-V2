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
    public class PostCategoryValidator : AbstractValidator<PostCategoryRequest>
    {
        private const int MAX_BYTES = 5242880;
        public PostCategoryValidator()
        {
            #region Code
            RuleFor(c => c.Code)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .MaximumLength(20).WithMessage("{PropertyName} is required less than or equal to 20 characters.");
            #endregion

            #region Name
            RuleFor(c => c.Name)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");
            #endregion

            #region Type
            RuleFor(c => c.Type)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckCategoryType).WithMessage("{PropertyName} is required NORMAL or EXTRA");
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
                   .NotNull().WithMessage("{PropertyName} is not null.")
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
