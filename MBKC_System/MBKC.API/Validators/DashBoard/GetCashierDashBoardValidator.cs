using FluentValidation;
using MBKC.Service.DTOs.DashBoards.Brand;
using MBKC.Service.DTOs.DashBoards.Cashier;

namespace MBKC.API.Validators.DashBoard
{
    public class GetCashierDashBoardValidator : AbstractValidator<GetCashierDashBoardRequest>
    {
        public GetCashierDashBoardValidator()
        {
            RuleFor(cd => cd.OrderSearchDateFrom)
                     .Cascade(CascadeMode.Stop)
                     .Matches(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[012])\/(19|20|)\d\d$")
                     .WithMessage("{PropertyName} is required dd/MM/yyyy");

            RuleFor(cd => cd.OrderSearchDateTo)
                     .Cascade(CascadeMode.Stop)
                     .Matches(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[012])\/(19|20|)\d\d$")
                     .WithMessage("{PropertyName} is required dd/MM/yyyy");

            RuleFor(cd => cd)
              .Cascade(CascadeMode.Stop)
              .Custom((cashierDashboard, context) =>
              {
                  if (string.IsNullOrWhiteSpace(cashierDashboard.OrderSearchDateFrom) == false && string.IsNullOrWhiteSpace(cashierDashboard.OrderSearchDateTo) == false)
                  {
                      DateTime dateFrom = new DateTime();
                      DateTime dateTo = new DateTime();
                      dateFrom = DateTime.ParseExact(cashierDashboard.OrderSearchDateFrom, "dd/MM/yyyy", null);
                      dateTo = DateTime.ParseExact(cashierDashboard.OrderSearchDateTo, "dd/MM/yyyy", null);
                      if (dateTo.Date < dateFrom.Date)
                      {
                          context.AddFailure("Search datetime", "OrderSearchDateTo must be greater than or equal to OrderSearchDateFrom.");
                      }
                  }
              });
        }
    }
}
