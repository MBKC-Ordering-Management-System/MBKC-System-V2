using FluentValidation;
using MBKC.Service.Constants;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.StorePartners;

namespace MBKC.API.Validators.StorePartners
{
    public class GetPartnerInformationValidator : AbstractValidator<GetPartnerInformationRequest>
    {
        public GetPartnerInformationValidator()
        {
            #region KeySortName
            RuleFor(x => x.keySortName)
                      .Cascade(CascadeMode.StopOnFirstFailure)
                      .Must((x, keySortName) =>
                               {
                                  if (keySortName == null)
                                  {
                                   return true; // Skip validation when keySortName is null
                                  }

        return keySortName.ToUpper().Equals(StorePartnerEnum.KeySort.ASC.ToString()) ||
               keySortName.ToUpper().Equals(StorePartnerEnum.KeySort.DESC.ToString());
    })
    .WithMessage(MessageConstant.StorePartnerMessage.KeySortNotExist);
            #endregion

            #region KeySortStatus
            RuleFor(x => x.keySortStatus)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .Must((x, keySortStatus) =>
                        {
                            if (keySortStatus == null)
                            {
                                return true; // Skip validation when keySortName is null
                            }

                            return keySortStatus.ToUpper().Equals(StorePartnerEnum.KeySort.ASC.ToString()) ||
                                   keySortStatus.ToUpper().Equals(StorePartnerEnum.KeySort.DESC.ToString());
                        }).WithMessage(MessageConstant.StorePartnerMessage.KeySortNotExist);
            #endregion

            #region keySortCommission
            RuleFor(x => x.keySortCommission)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .Must((x, keySortCommission) =>
                        {
                            if (keySortCommission == null)
                            {
                                return true; // Skip validation when keySortName is null
                            }

                            return keySortCommission.ToUpper().Equals(StorePartnerEnum.KeySort.ASC.ToString()) ||
                                 keySortCommission.ToUpper().Equals(StorePartnerEnum.KeySort.DESC.ToString());
                        }).WithMessage(MessageConstant.StorePartnerMessage.KeySortNotExist);
            #endregion
        }
    }
}
