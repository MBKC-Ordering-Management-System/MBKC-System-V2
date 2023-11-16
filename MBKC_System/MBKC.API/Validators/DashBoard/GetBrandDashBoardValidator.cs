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
                  .NotNull().WithMessage("{PropertyName} is not null.")
                  .NotEmpty().WithMessage("{PropertyName} is not empty.")
                  .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

            RuleFor(bd => bd.ProductSearchDateFrom)
                     .Cascade(CascadeMode.Stop)
                     .Matches(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[012])\/(19|20|)\d\d$")
                     .WithMessage("{PropertyName} is required dd/MM/yyyy");

            RuleFor(bd => bd.ProductSearchDateTo)
                     .Cascade(CascadeMode.Stop)
                     .Matches(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[012])\/(19|20|)\d\d$")
                     .WithMessage("{PropertyName} is required dd/MM/yyyy");

            RuleFor(bd => bd)
              .Cascade(CascadeMode.Stop)
              .Custom((brandDashboard, context) =>
              {
                  if (string.IsNullOrWhiteSpace(brandDashboard.ProductSearchDateFrom) == false && string.IsNullOrWhiteSpace(brandDashboard.ProductSearchDateTo) == false)
                  {
                      DateTime dateFrom = new DateTime();
                      DateTime dateTo = new DateTime();
                      dateFrom = DateTime.ParseExact(brandDashboard.ProductSearchDateFrom, "dd/MM/yyyy", null);
                      dateTo = DateTime.ParseExact(brandDashboard.ProductSearchDateTo, "dd/MM/yyyy", null);
                      if (dateTo.Date < dateFrom.Date)
                      {
                          context.AddFailure("Search datetime", "ProductSearchDateTo must be greater than or equal to ProductSearchDateFrom.");
                      }
                  }
              });
        }

    }
}
