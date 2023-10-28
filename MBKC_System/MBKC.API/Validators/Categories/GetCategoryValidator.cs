using FluentValidation;
using MBKC.Service.DTOs.Categories;

namespace MBKC.API.Validators.Categories
{
    public class GetCategoryValidator : AbstractValidator<CategoryRequest>
    {
        public GetCategoryValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}
