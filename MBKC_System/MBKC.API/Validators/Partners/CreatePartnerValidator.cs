using FluentValidation;
using MBKC.Service.DTOs.Partners;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Partners
{
    public class CreatePartnerValidator : AbstractValidator<PostPartnerRequest>
    {
        private const int MAX_BYTES = 2048000;
        public CreatePartnerValidator()
        {
            #region Name
            RuleFor(p => p.Name)
                         .Cascade(CascadeMode.StopOnFirstFailure)
                         .NotNull().WithMessage("{PropertyName} is null.")
                         .NotEmpty().WithMessage("{PropertyName} is empty.")
                         .MaximumLength(50).WithMessage("{PropertyName} is required less than or equal to 50 characters.");

            #endregion

            #region Logo
            RuleFor(b => b.Logo)
                       .Cascade(CascadeMode.StopOnFirstFailure)
                       .ChildRules(pro => pro.RuleFor(img => img.Length).ExclusiveBetween(0, MAX_BYTES)
                       .WithMessage($"Logo is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB."));
            RuleFor(p => p.Logo)
                   .ChildRules(pro => pro.RuleFor(img => img.FileName).Must(FileUtil.HaveSupportedFileType).WithMessage("Logo is required extension type .png, .jpg, .jpeg, .webp."));
            #endregion

            #region WebUrl
            RuleFor(p => p.WebUrl)
                         .Cascade(CascadeMode.StopOnFirstFailure)
                         .MaximumLength(150).WithMessage("{PropertyName} is required less than or equal to 150 characters.");

            #endregion
        }

    }
}
