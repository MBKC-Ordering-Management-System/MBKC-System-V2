using FluentValidation;
using MBKC.BAL.DTOs.Categories;
using MBKC.BAL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Validators.Categories
{
    public class PostCategoryValidation : AbstractValidator<PostCategoryRequest>
    {
        private const int MAX_BYTES = 2048000;
        public PostCategoryValidation()
        {
            /*public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int DisplayOrder { get; set; }
        public string Description { get; set; }
        public int BrandId { get; set; }
        public IFormFile ImageUrl { get; set; }*/

            #region Code
            RuleFor(c => c.Code)
                     .NotEmpty().WithMessage("{PropertyName} is empty.")
                     .NotNull().WithMessage("{PropertyName} is null.")
                     .Length(1, 20).WithMessage("{PropertyName} from {MinLength} to {MaxLength} characters.");
            #endregion

            #region Name
            RuleFor(c => c.Name)
                     .NotEmpty().WithMessage("{PropertyName} is empty.")
                     .NotNull().WithMessage("{PropertyName} is null.")
                     .Length(1, 100).WithMessage("{PropertyName} from {MinLength} to {MaxLength} characters.");
            #endregion

            #region Type
            RuleFor(c => c.Type)
                     .NotEmpty().WithMessage("{PropertyName} is empty.")
                     .NotNull().WithMessage("{PropertyName} is null.")
                     .Length(1, 20).WithMessage("{PropertyName} from {MinLength} to {MaxLength} characters.");
            #endregion

            #region Description
            RuleFor(c => c.Description)
                     .NotEmpty().WithMessage("{PropertyName} is empty.")
                     .NotNull().WithMessage("{PropertyName} is null.")
                     .Length(1, 100).WithMessage("{PropertyName} from {MinLength} to {MaxLength} characters.");
            #endregion

            #region Logo
            RuleFor(c => c.ImageUrl)
                   .ChildRules(category => category.RuleFor(img => img.Length).ExclusiveBetween(0, MAX_BYTES)
                   .WithMessage($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB"));
            RuleFor(c => c.ImageUrl)
                   .ChildRules(pro => pro.RuleFor(img => img.FileName).Must(FileUtil.HaveSupportedFileType).WithMessage("Image is required extension type .png, .jpg, .jpeg"));
            #endregion
        }
    }
}
