using FluentValidation;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.DTOs.Partners;

namespace MBKC.API.Validators.Orders
{
    public class ConfirmOrderToCompletedValidator : AbstractValidator<ConfirmOrderToCompletedRequest>
    {
        public ConfirmOrderToCompletedValidator()
        {
            RuleFor(p => p.OrderPartnerId)
                         .Cascade(CascadeMode.StopOnFirstFailure)
                         .NotNull().WithMessage("{PropertyName} is null.")
                         .NotEmpty().WithMessage("{PropertyName} is empty.")
                         .MaximumLength(100).WithMessage("{PropertyName} is required less than or equal to 100 characters.");

            RuleFor(p => p.ShipperPhone)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull().WithMessage("{PropertyName} is null.")
                        .NotEmpty().WithMessage("{PropertyName} is empty.")
                        .Matches(@"^[0-9]+$").WithMessage("{PropertyName} contain only number.")
                        .Length(10).WithMessage("{PropertyName} must be 10 characters.");

            RuleFor(p => p.BankingAccountId)
                      .Cascade(CascadeMode.StopOnFirstFailure)
                      .GreaterThan(0).WithMessage("{PropertyName} must greater than 0.");
        }
    }
}
