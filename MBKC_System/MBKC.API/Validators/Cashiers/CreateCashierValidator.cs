using FluentValidation;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Cashiers.Requests;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Cashiers
{
    public class CreateCashierValidator: AbstractValidator<CreateCashierRequest>
    {
        private const int MAX_BYTES = 5242880;
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
                    if(DateTime.Now.Year - dateOfBrith.Year < 18 || DateTime.Now.Year - 55 > dateOfBrith.Year)
                    {
                        context.AddFailure("DateOfBirth", "Cashier's age is required from 18 to 55 years old.");
                    }
                });

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email format.");

            RuleFor(x => x.CitizenNumber)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Length(12, 12).WithMessage("{PropertyName} is required 12 digits.")
                .Must(StringUtil.IsDigitString).WithMessage("{PropertyName} only contains digits.");

            RuleFor(x => x.Avatar)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .ChildRules(x =>
                {
                    x.RuleFor(x => x.Length)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .ExclusiveBetween(0, MAX_BYTES).WithMessage($"Avatar is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB.");

                    x.RuleFor(x => x.FileName)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .Must(FileUtil.HaveSupportedFileType).WithMessage("Avatar is required extension type .png, .jpg, .jpeg, .webp.");
                });
        }
    }
}
