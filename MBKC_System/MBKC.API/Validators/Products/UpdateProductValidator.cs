using FluentValidation;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Products;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Products
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
    {
        private const int MAX_BYTES = 5242880;
        public UpdateProductValidator()
        {
            RuleFor(cpr => cpr.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .MaximumLength(120).WithMessage("{PropertyName} is required less than or equal to 120 characters.");

            RuleFor(cpr => cpr.Description)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(1000).WithMessage("{PropertyName} is required less than or equal to 1000 characters.");

            RuleFor(cpr => cpr.Image)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(ckcr =>
                {
                    ckcr.RuleFor(cpr => cpr.Length)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .ExclusiveBetween(0, MAX_BYTES).WithMessage($"Image is required file length greater than 0 and less than {MAX_BYTES / 1024 / 1024} MB.");

                    ckcr.RuleFor(cpr => cpr.FileName)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .Must(FileUtil.HaveSupportedFileType).WithMessage("Image is required extension type .png, .jpg, .jpeg, .webp.");
                });

            RuleFor(cpr => cpr.DisplayOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is required greater than 0.");

            RuleFor(cpr => cpr.SellingPrice)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(prop => prop.RuleFor(sellignPrice => sellignPrice).GreaterThanOrEqualTo(0).WithMessage("Selling price is required greater than or equal to 0."));


            RuleFor(cpr => cpr.DiscountPrice)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(prop => prop.RuleFor(sellignPrice => sellignPrice).GreaterThanOrEqualTo(0).WithMessage("Discount price is required greater than or equal to 0."));

            RuleFor(cpr => cpr.HistoricalPrice)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .ChildRules(prop => prop.RuleFor(sellignPrice => sellignPrice).GreaterThanOrEqualTo(0).WithMessage("Historical price is required greater than or equal to 0."));

            RuleFor(cpr => cpr.ParentProductId)
                .ChildRules(prop => prop.RuleFor(parentProductId => parentProductId)
                                             .Cascade(CascadeMode.StopOnFirstFailure)
                                             .GreaterThan(0).WithMessage("{PropertyName} is not suiltable id in the system."));

            RuleFor(cpr => cpr.Status)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .IsEnumName(typeof(ProductEnum.Status), caseSensitive: false).WithMessage("{PropertyName} is required some statuses such as: INACTIVE, ACTIVE.")
                .ChildRules(x => x.RuleFor(x => x.ToLower())
                                         .Cascade(CascadeMode.StopOnFirstFailure)
                                         .NotEqual(ProductEnum.Status.DISABLE.ToString().ToLower()).WithMessage("Status is required some statuses such as: INACTIVE, ACTIVE."));

            RuleFor(cpr => cpr.CategoryId)
                .ChildRules(prop => prop.RuleFor(categoryId => categoryId)
                                             .Cascade(CascadeMode.StopOnFirstFailure)
                                             .GreaterThan(0).WithMessage("{PropertyName} is not suiltable id in the system."));
            RuleFor(cpr => cpr)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Custom((product, context) =>
                {
                    if (product != null)
                    {
                        if (product.SellingPrice < product.HistoricalPrice)
                        {
                            context.AddFailure("SellingPrice", "Selling price is required greater than or equal to Historical price.");
                        }
                        if (product.SellingPrice < product.DiscountPrice)
                        {
                            context.AddFailure("DiscountPrice", "Discount price is required less than or equal to Selling price.");
                        }
                    }
                });
        }
    }
}
