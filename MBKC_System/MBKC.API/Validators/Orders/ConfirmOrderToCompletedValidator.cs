using FluentValidation;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.DTOs.Partners;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Orders
{
    public class ConfirmOrderToCompletedValidator : AbstractValidator<ConfirmOrderToCompletedRequest>
    {
        private const int MAX_BYTES = 5242880;
        public ConfirmOrderToCompletedValidator()
        {
            RuleFor(p => p.OrderPartnerId)
                         .Cascade(CascadeMode.Stop)
                         .NotNull().WithMessage("{PropertyName} is null.")
                         .NotEmpty().WithMessage("{PropertyName} is empty.")
                         .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");

            RuleFor(p => p.BankingAccountId)
                      .Cascade(CascadeMode.Stop)
                      .GreaterThan(0).WithMessage("{PropertyName} must greater than 0.");

            RuleFor(p => p.Image)
               .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("{PropertyName} is null.")
               .ChildRules(p =>
               {
                   p.RuleFor(p => p.Length)
                       .Cascade(CascadeMode.Stop)
                       .ExclusiveBetween(0, MAX_BYTES).WithMessage($"Image is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB.");

                   p.RuleFor(p => p.FileName)
                       .Cascade(CascadeMode.Stop)
                       .Must(FileUtil.HaveSupportedFileType).WithMessage("{PropertyName} is required extension type .png, .jpg, .jpeg, .webp.");
               });
        }
    }
}
