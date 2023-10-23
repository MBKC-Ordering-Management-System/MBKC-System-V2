using FluentValidation;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Stores
{
    public class UpdateStoreStatusValidator: AbstractValidator<UpdateStoreStatusRequest>
    {
        public UpdateStoreStatusValidator()
        {
            RuleFor(ussr => ussr.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckStoreStatusName).WithMessage("{PropertyName} is required 'Active' or 'InActive' Status.");
        }
    }
}
