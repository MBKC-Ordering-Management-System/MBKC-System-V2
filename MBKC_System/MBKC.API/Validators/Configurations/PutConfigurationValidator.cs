using FluentValidation;
using MBKC.Service.DTOs.Configurations;

namespace MBKC.API.Validators.Configurations
{
    public class PutConfigurationValidator:AbstractValidator<PutConfigurationRequest>
    {
        public PutConfigurationValidator()
        {
            RuleFor(x => x.ScrawlingOrderStartTime)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Matches(@"^([01]\d?|2[0-4]):[0-5]\d(:[0-5]\d)?$").WithMessage("{PropertyName} is invalid time (HH:mm:ss).");

            RuleFor(x => x.ScrawlingOrderEndTime)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .Matches(@"^([01]\d?|2[0-4]):[0-5]\d(:[0-5]\d)?$").WithMessage("{PropertyName} is invalid time (HH:mm:ss).");

            RuleFor(x => x)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Custom((configuration, context) =>
                {
                    TimeSpan startTime;
                    TimeSpan endTime;
                    if (string.IsNullOrWhiteSpace(configuration.ScrawlingOrderStartTime) == false && string.IsNullOrWhiteSpace(configuration.ScrawlingOrderEndTime) == false)
                    {
                        TimeSpan.TryParse(configuration.ScrawlingOrderStartTime, out startTime);
                        TimeSpan.TryParse(configuration.ScrawlingOrderEndTime, out endTime);
                        if(TimeSpan.Compare(startTime, endTime) > 0)
                        {
                            context.AddFailure("ScrawlingOrderStartTime", "Scrawling order start time is required less than or equal to Scrawling order end time.");
                        }
                    }
                });
        }
    }
}
