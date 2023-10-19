using FluentValidation;
using MBKC.Service.DTOs.StorePartners;

namespace MBKC.API.Validators.StorePartners
{
    public class CreateStorePartnerValidator : AbstractValidator<PostStorePartnerRequest>
    {
        public CreateStorePartnerValidator()
        {
            RuleForEach(x => x.partnerAccounts)
                .ChildRules(x => x.RuleFor(x => x.PartnerId)
                                   .Cascade(CascadeMode.StopOnFirstFailure)
                                   .NotNull().WithMessage("{PropertyName} is not null.")
                                   .GreaterThan(0).WithMessage("{PropertyName} is not suitable in the system."));
            #region UserName
            RuleForEach(storePartner => storePartner.partnerAccounts)
                .ChildRules(partnerAccount => partnerAccount.RuleFor(username => username.UserName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less then or equal to 100 characters."));


            #endregion

            #region Password
            RuleForEach(storePartner => storePartner.partnerAccounts)
                .ChildRules(partnerAccount => partnerAccount.RuleFor(username => username.Password)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(50).WithMessage("{PropertyName} is required less then or equal to 50 characters."));
            #endregion

            RuleFor(x => x.StoreId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

            RuleFor(x => x.IsMappingProducts)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");
        }
    }
}
