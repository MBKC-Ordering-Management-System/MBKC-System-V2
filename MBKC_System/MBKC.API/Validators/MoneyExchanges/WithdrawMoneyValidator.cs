using FluentValidation;
using MBKC.Service.DTOs.MoneyExchanges;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.MoneyExchanges
{
    public class WithdrawMoneyValidator : AbstractValidator<WithdrawMoneyRequest>
    {
        private const int MAX_BYTES = 5242880;
        public WithdrawMoneyValidator()
        {
            RuleFor(wm => wm.StoreId)
                         .Cascade(CascadeMode.Stop)
                         .NotNull().WithMessage("{PropertyName} is null.")
                         .NotEmpty().WithMessage("{PropertyName} is empty.")
                         .GreaterThan(0).WithMessage("{PropertyName} must greater than 0.");

            RuleFor(wm => wm.Amount)
                   .Cascade(CascadeMode.Stop)     
                   .NotNull().WithMessage("{PropertyName} is null.")
                   .NotEmpty().WithMessage("{PropertyName} is empty.")
                   .InclusiveBetween(1000, 999999999).WithMessage("{PropertyName} min is 1.000đ and max is 999.999.999đ.");

            RuleFor(wm => wm.Image)
               .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("{PropertyName} is not null.")
               .ChildRules(wm =>
               {
                   wm.RuleFor(wm => wm.Length)
                       .Cascade(CascadeMode.Stop)
                       .ExclusiveBetween(0, MAX_BYTES).WithMessage($"Image is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB.");

                   wm.RuleFor(wm => wm.FileName)
                       .Cascade(CascadeMode.Stop)
                       .Must(FileUtil.HaveSupportedFileType).WithMessage("{PropertyName} is required extension type .png, .jpg, .jpeg, .webp.");
               });

        }
    }
}
