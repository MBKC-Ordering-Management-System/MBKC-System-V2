using FluentValidation;
using MBKC.Service.DTOs.DashBoards.Brand;
using MBKC.Service.DTOs.Orders;

namespace MBKC.API.Validators.DashBoard
{
    public class GetBrandDashBoardValidator : AbstractValidator<GetBrandDashBoardRequest>
    {

        public GetBrandDashBoardValidator()
        {
            RuleFor(bd => bd.StoreId)
                  .Cascade(CascadeMode.Stop)
                  .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

        }

    }
}
