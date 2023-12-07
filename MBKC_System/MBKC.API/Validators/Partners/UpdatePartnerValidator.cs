using FluentValidation;
using MBKC.Service.Utils;
using MBKC.Service.DTOs.Partners;

namespace MBKC.API.Validators.Partners
{
    public class UpdatePartnerValidator : AbstractValidator<UpdatePartnerRequest>
    {
        private const int MAX_BYTES = 2048000;
        public UpdatePartnerValidator()
        {

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
                         .NotNull().WithMessage("{PropertyName} is null.")
                         .NotEmpty().WithMessage("{PropertyName} is empty.")
                         .MaximumLength(150).WithMessage("{PropertyName} is required less than or equal to 150 characters.");
            #endregion

            #region Status
            RuleFor(b => b.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure) 
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckPartnerStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
            #endregion

             RuleFor(p => p.TaxCommission)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .InclusiveBetween(0, 100).WithMessage("{PropertyName} must be between 0% and 100%.");
        }

    }
}
