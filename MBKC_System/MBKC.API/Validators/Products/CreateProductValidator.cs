using FluentValidation;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Products;
using MBKC.Service.Utils;
using System.Runtime.ConstrainedExecution;

namespace MBKC.API.Validators.Products
{
    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        private const int MAX_BYTES = 5242880;
        public CreateProductValidator()
        {
            RuleFor(cpr => cpr.Code)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(20).WithMessage("{PropertyName} is required less than or equal to 20 characters.");

            RuleFor(cpr => cpr.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(120).WithMessage("{PropertyName} is required less than or equal to 120 characters.");

            RuleFor(cpr => cpr.Description)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(1000).WithMessage("{PropertyName} is required less than or equal to 1000 characters.");

            RuleFor(cpr => cpr.Type)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(20).WithMessage("{PropertyName} is required less than or equal to 20 characters.")
                .IsEnumName(typeof(ProductEnum.Type), caseSensitive: false).WithMessage("{PropertyName} is required some types such as: SINGLE, PARENT, CHILD, EXTRA.");

            RuleFor(cpr => cpr.Image)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
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

            RuleFor(cpr => cpr.Size)
                .ChildRules(prop => prop.RuleFor(size => size)
                                            .Cascade(CascadeMode.StopOnFirstFailure)
                                            .IsEnumName(typeof(ProductEnum.Size), caseSensitive: false).WithMessage("Size is required some types such as: S, M, L."));

            RuleFor(cpr => cpr.ParentProductId)
                .ChildRules(prop => prop.RuleFor(parentProductId => parentProductId)
                                             .Cascade(CascadeMode.StopOnFirstFailure)
                                             .GreaterThanOrEqualTo(0).WithMessage("Parent product id is not suitable id in the system."));



            RuleFor(cpr => cpr.CategoryId)
                .ChildRules(prop => prop.RuleFor(categoryId => categoryId)
                                             .Cascade(CascadeMode.StopOnFirstFailure)
                                             .GreaterThanOrEqualTo(0).WithMessage("Category id is not suitable id in the system."));


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
                    if (product != null && product.Type != null && product.Type.Trim().ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()))
                    {
                        if (product.Size != null)
                        {
                            context.AddFailure("Size", "Size is not required for SINGLE type.");
                        }
                        if (product.ParentProductId != null)
                        {
                            context.AddFailure("ParentProductId", "Parent product id is not required for SINGLE type.");
                        }
                    }

                    if (product != null && product.Type != null && product.Type.Trim().ToLower().Equals(ProductEnum.Type.EXTRA.ToString().ToLower()))
                    {
                        if (product.Size != null)
                        {
                            context.AddFailure("Size", "Size is not required for EXTRA type.");
                        }
                        if (product.ParentProductId != null)
                        {
                            context.AddFailure("ParentProductId", "Parent product id is not required for EXTRA type.");
                        }
                    }

                    if (product != null && product.Type != null && product.Type.Trim().ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                    {
                        if (product.Size != null)
                        {
                            context.AddFailure("Size", "Size is not required for PARENT type.");
                        }
                        if (product.ParentProductId != null)
                        {
                            context.AddFailure("ParentProductId", "Parent product id is not required for PARENT type.");
                        }

                        if (product.SellingPrice != null)
                        {
                            context.AddFailure("SellingPrice", "Selling price is not required for PARENT type.");
                        }

                        if (product.DiscountPrice != null)
                        {
                            context.AddFailure("DiscountPrice", "Discount price is not required for PARENT type.");
                        }

                        if (product.HistoricalPrice != null)
                        {
                            context.AddFailure("HistoricalPrice", "Historical price is not required for PARENT type.");
                        }
                    }

                    if (product != null && product.Type != null && product.Type.Trim().ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))
                    {
                        if (product.CategoryId != null)
                        {
                            context.AddFailure("CategoryId", "Category id is not required for CHILD type.");
                        }
                    }
                });
        }
    }
}
