using FluentValidation;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.API.Validators.Brands
{
    public class UpdateBrandStatusValidator: AbstractValidator<UpdateBrandStatusRequest>
    {
        public UpdateBrandStatusValidator()
        {
            RuleFor(ubsr => ubsr.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .NotNull().WithMessage("{PropertyName} is not null.")
                     .NotEmpty().WithMessage("{PropertyName} is not empty.")
                     .Must(StringUtil.CheckBrandStatusName).WithMessage("{PropertyName} is required INACTIVE or ACTIVE");
        }
    }
}
