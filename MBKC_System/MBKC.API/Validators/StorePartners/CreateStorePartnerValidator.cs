using FluentValidation;
using MBKC.Service.DTOs.StorePartners;

namespace MBKC.API.Validators.StorePartners
{
    public class CreateStorePartnerValidator : AbstractValidator<PostStorePartnerRequest>
    {
        public CreateStorePartnerValidator()
        {
            #region StoreId

            RuleFor(storePartner => storePartner.StoreId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable in the system.");
            #endregion

            #region PartnerId
            RuleForEach(storePartner => storePartner.PartnerAccounts)
                .ChildRules(partnerAccount => partnerAccount.RuleFor(partnerId => partnerId.PartnerId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable in the system."));
            #endregion

            #region UserName
            RuleForEach(storePartner => storePartner.PartnerAccounts)
                .ChildRules(partnerAccount => partnerAccount.RuleFor(username => username.UserName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(100).WithMessage("{PropertyName} is required less then or equal to 100 characters."));
            #endregion

            #region Password
            RuleForEach(storePartner => storePartner.PartnerAccounts)
                .ChildRules(partnerAccount => partnerAccount.RuleFor(password => password.Password)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .MaximumLength(50).WithMessage("{PropertyName} is required less then or equal to 50 characters."));
            #endregion

            #region Commission
            RuleForEach(storePartner => storePartner.PartnerAccounts)
                .ChildRules(partnerAccount => partnerAccount.RuleFor(commission => commission.Commission)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .InclusiveBetween(0, 100).WithMessage("{PropertyName} must be between 0% and 100%."));
            #endregion

            RuleFor(x => x.IsMappingProducts)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");
        }
    }
}
