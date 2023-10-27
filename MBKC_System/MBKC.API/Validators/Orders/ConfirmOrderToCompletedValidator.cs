﻿using FluentValidation;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.DTOs.Partners;

namespace MBKC.API.Validators.Orders
{
    public class ConfirmOrderToCompletedValidator : AbstractValidator<ConfirmOrderToCompletedRequest>
    {
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
        }
    }
}
