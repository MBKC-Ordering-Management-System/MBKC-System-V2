using FluentValidation;
using MBKC.Service.DTOs.KitchenCenters;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.API.Validators.KitchenCenters
{
    public class UpdateKitchenCenterStatusValidator: AbstractValidator<UpdateKitchenCenterStatusRequest>
    {
        public UpdateKitchenCenterStatusValidator()
        {
            RuleFor(ubsr => ubsr.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckKitchenCenterStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
        }
    }
}
