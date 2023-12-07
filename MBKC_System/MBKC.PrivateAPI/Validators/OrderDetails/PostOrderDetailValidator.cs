using FluentValidation;
using MBKC.Service.DTOs.OrderDetails;

namespace MBKC.PrivateAPI.Validators.OrderDetails
{
    public class PostOrderDetailValidator : AbstractValidator<PostOrderDetailRequest>
    {
        public PostOrderDetailValidator()
        {
            RuleFor(x => x.ProductId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable Id in the system.");

            RuleFor(x => x.SellingPrice)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} is required greater than or equal to 0 VNĐ.");

            RuleFor(x => x.DiscountPrice)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} is required greater than or equal to 0 VNĐ.");

            RuleFor(x => x)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Custom((request, context) =>
                {
                    if (request.DiscountPrice > request.SellingPrice)
                    {
                        context.AddFailure("Discount price", "Discount price is required less than or equal to Selling price.");
                    }
                });

            RuleFor(x => x.Quantity)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is required greater than 0.");

            RuleFor(x => x.Note)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");

            RuleForEach(x => x.ExtraOrderDetails)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(x =>
                {
                    x.RuleFor(x => x.ProductId)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull().WithMessage("{PropertyName} is not null.")
                        .GreaterThan(0).WithMessage("{PropertyName} is not suitable Id in the system.");

                    x.RuleFor(x => x.SellingPrice)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull().WithMessage("{PropertyName} is not null.")
                        .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} is required greater than or equal to 0 VNĐ.");

                    x.RuleFor(x => x.DiscountPrice)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull().WithMessage("{PropertyName} is not null.")
                        .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} is required greater than or equal to 0 VNĐ.");

                    x.RuleFor(x => x.Quantity)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull().WithMessage("{PropertyName} is not null.")
                        .GreaterThan(0).WithMessage("{PropertyName} is required greater than 0.");

                    x.RuleFor(x => x.Note)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull().WithMessage("{PropertyName} is not null.");

                    x.RuleFor(x => x)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .Custom((request, context) =>
                        {
                            if (request.DiscountPrice > request.SellingPrice)
                            {
                                context.AddFailure("Discount price", "Discount price is required less than or equal to Selling price.");
                            }
                        });
                });
        }
    }
}
