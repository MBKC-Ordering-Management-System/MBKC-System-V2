using FluentValidation;
using MBKC.Service.DTOs.MoneyExchanges;

namespace MBKC.API.Validators.MoneyExchanges
{
    public class UpdateCronJobValidator : AbstractValidator<UpdateCronJobRequest>
    {
        public UpdateCronJobValidator()
        {
            RuleFor(uc => uc.JobId)
                         .Cascade(CascadeMode.Stop)
                         .NotNull().WithMessage("{PropertyName} is null.")
                         .NotEmpty().WithMessage("{PropertyName} is empty.");

            RuleFor(uc => uc.hour)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("{PropertyName} is null.")
                   .InclusiveBetween(0, 23).WithMessage("{PropertyName} min is 0 and max 23.");

            RuleFor(uc => uc.minute)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("{PropertyName} is null.")
                 .InclusiveBetween(0, 59).WithMessage("{PropertyName} min is 0 and max is 59.");
        }
    }
}
