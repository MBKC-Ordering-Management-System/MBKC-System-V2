using FluentValidation;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Cashiers.Requests;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Cashiers
{
    public class UpdateCashierValidator: AbstractValidator<UpdateCashierRequest>
    {
        private const int MAX_BYTES = 5242880;
        public UpdateCashierValidator()
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
                    if (DateTime.Now.Year - dateOfBrith.Year < 18 || DateTime.Now.Year - 55 > dateOfBrith.Year)
                    {
                        context.AddFailure("DateOfBirth", "Cashier's age is required from 18 to 55 years old.");
                    }
                });

            RuleFor(x => x.CitizenNumber)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(12).WithMessage("{PropertyName} is required less than or equal to 12 digits.")
                .Must(StringUtil.IsDigitString).WithMessage("{PropertyName} only contains digits.");

            RuleFor(x => x.Avatar)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(x =>
                {
                    x.RuleFor(x => x.Length)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .ExclusiveBetween(0, MAX_BYTES).WithMessage($"Avatar is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB.");

                    x.RuleFor(x => x.FileName)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .Must(FileUtil.HaveSupportedFileType).WithMessage("Avatar is required extension type .png, .jpg, .jpeg, .webp.");
                });

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(x => x.RuleFor(x => x).Cascade(CascadeMode.StopOnFirstFailure)
                                                  .NotEmpty().WithMessage("New password is not empty.")
                                                  .MaximumLength(50).WithMessage("New password is required less than or equal to 50 characters.")
                                                  .Must(StringUtil.IsMD5).WithMessage("New password must convert with MD5 althorigm before saving in the system."));

            RuleFor(cpr => cpr.Status)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(x => x.RuleFor(x => x.ToLower())
                                         .Cascade(CascadeMode.StopOnFirstFailure)
                                         .IsEnumName(typeof(ProductEnum.Status), caseSensitive: false).WithMessage("{PropertyName} is required some statuses such as: INACTIVE, ACTIVE.")
                                         .NotEqual(ProductEnum.Status.DISABLE.ToString().ToLower()).WithMessage("Status is required some statuses such as: INACTIVE, ACTIVE."));
        }
    }
}
