using FluentValidation;
using MBKC.Service.DTOs.Stores;

namespace MBKC.API.Validators.Stores
{
    public class GetStoreValidator : AbstractValidator<StoreRequest>
    {
        public GetStoreValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");
        }
    }
}

