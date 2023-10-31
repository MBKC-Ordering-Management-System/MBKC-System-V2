using FluentValidation;
using MBKC.Service.DTOs.Products;

namespace MBKC.API.Validators.Products
{
    public class GetProductValidator : AbstractValidator<ProductRequest>
    {
        public GetProductValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
