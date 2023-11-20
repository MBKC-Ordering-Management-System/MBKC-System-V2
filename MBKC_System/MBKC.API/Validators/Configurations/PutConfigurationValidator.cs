using FluentValidation;
using MBKC.Service.DTOs.Configurations;
using MBKC.Service.Exceptions;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Configurations
{
    public class PutConfigurationValidator : AbstractValidator<PutConfigurationRequest>
    {
        public PutConfigurationValidator()
        {
            RuleFor(x => x.ScrawlingOrderStartTime)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Matches(@"^([01]\d?|2[0-4]):[0-5]\d(:[0-5]\d)?$").WithMessage("{PropertyName} is invalid time (HH:mm:ss).");

            RuleFor(x => x.ScrawlingOrderEndTime)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Matches(@"^([01]\d?|2[0-4]):[0-5]\d(:[0-5]\d)?$").WithMessage("{PropertyName} is invalid time (HH:mm:ss).");

            RuleFor(x => x.ScrawlingMoneyExchangeToKitchenCenter)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Matches(@"^([01]\d?|2[0-4]):[0-5]\d(:[0-5]\d)?$").WithMessage("{PropertyName} is invalid time (HH:mm:ss).");

            RuleFor(x => x.ScrawlingMoneyExchangeToStore)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Matches(@"^([01]\d?|2[0-4]):[0-5]\d(:[0-5]\d)?$").WithMessage("{PropertyName} is invalid time (HH:mm:ss).");

            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .Custom((configuration, context) =>
                {
                    TimeSpan startTime;
                    TimeSpan endTime;
                    if (string.IsNullOrWhiteSpace(configuration.ScrawlingOrderStartTime) == false && string.IsNullOrWhiteSpace(configuration.ScrawlingOrderEndTime) == false)
                    {
                        TimeSpan.TryParse(configuration.ScrawlingOrderStartTime, out startTime);
                        TimeSpan.TryParse(configuration.ScrawlingOrderEndTime, out endTime);
                        if (TimeSpan.Compare(startTime, endTime) > 0)
                        {
                            context.AddFailure("ScrawlingOrderStartTime", "Scrawling order start time is required less than or equal to Scrawling order end time.");
                        }
                    }
                });

            RuleFor(x => x)
               .Cascade(CascadeMode.Stop)
               .Custom((configuration, context) =>
               {
                   TimeSpan exchangeToKitchenCenter;
                   TimeSpan exchangeToStore;
                   if (string.IsNullOrWhiteSpace(configuration.ScrawlingMoneyExchangeToKitchenCenter) == false && string.IsNullOrWhiteSpace(configuration.ScrawlingMoneyExchangeToStore) == false)
                   {
                       TimeSpan.TryParse(configuration.ScrawlingMoneyExchangeToKitchenCenter, out exchangeToKitchenCenter);
                       TimeSpan.TryParse(configuration.ScrawlingMoneyExchangeToStore, out exchangeToStore);

                       if (DateUtil.IsTimeUpdateValid(exchangeToStore, exchangeToKitchenCenter, 1, DateUtil.TypeCheck.HOUR) == false)
                       {
                           context.AddFailure("ScrawlingExchangeToKitchenCenter", "The scheduling time of money transfer to the kitchen center must be at least 1 hour earlier than the money transfer time to the store.");
                       }
                   }
               });

            RuleFor(x => x)
           .Cascade(CascadeMode.Stop)
           .Custom((configuration, context) =>
           {
               TimeSpan endTime;
               TimeSpan exchangeToKitchenCenterTime;
               if (string.IsNullOrWhiteSpace(configuration.ScrawlingOrderEndTime) == false && string.IsNullOrWhiteSpace(configuration.ScrawlingMoneyExchangeToKitchenCenter) == false)
               {
                   TimeSpan.TryParse(configuration.ScrawlingOrderEndTime, out endTime);
                   TimeSpan.TryParse(configuration.ScrawlingMoneyExchangeToKitchenCenter, out exchangeToKitchenCenterTime);

                   if (DateUtil.IsTimeUpdateValid(exchangeToKitchenCenterTime, endTime, 10, DateUtil.TypeCheck.MINUTE) == false)
                   {
                       context.AddFailure("ScrawlingExchangeToKitchenCenter", "The Scrawling order end time must be at least 10 minute earlier than the money transfer time to the kitchen center.");
                   }
               }
           });

        }
    }
}
