using FluentValidation;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Cashiers;

namespace MBKC.API.Validators.Cashiers
{
    public class CreateCashierValidator: AbstractValidator<CreateCashierRequest>
    {
        public CreateCashierValidator()
        {
            RuleFor(x => x.FullName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(80).WithMessage("{PropertyName} is required less than or equal to 80 characters.");

            RuleFor(x => x.Gender)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .IsEnumName(typeof(CashierEnum.Gender), caseSensitive: false).WithMessage("{PropertyName} is required MALE or FEMALE.");

            RuleFor(x => x.DateOfBirth)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .Custom((dateOfBrith, context) =>
                {
                    if(DateTime.Now.Year - dateOfBrith.Year < 18)
                    {
                        /*context.AddFailure("DateOfBirth", "DateOfBirth is required ")*/
                    }
                });
        }
    }
}
