using FluentValidation;
using MBKC.Service.Utils;
using MBKC.Service.DTOs.MoneyExchanges;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MBKC.API.Validators.MoneyExchanges
{
    public class GetMoneyExchangesWithDrawValidator : AbstractValidator<GetMoneyExchangesWithDrawRequest>
    {
        public GetMoneyExchangesWithDrawValidator()
        {
            RuleFor(x => x.CurrentPage)
                .Cascade(CascadeMode.StopOnFirstFailure)
              .Custom((currentPage, context) =>
              {
                  if (currentPage <= 0)
                  {
                      context.AddFailure("CurrentPage", "Current page number is required more than 0.");
                  }
              });

            RuleFor(x => x.ItemsPerPage)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Custom((itemsPerPage, context) =>
                {
                    if (itemsPerPage <= 0)
                    {
                        context.AddFailure("ItemsPerPage", "Items per page number is required more than 0.");
                    }
                });

            RuleFor(x => x.SortBy)
                .Cascade(CascadeMode.StopOnFirstFailure)
                 .Custom((sortBy, context) =>
                 {
                     PropertyInfo[] properties = typeof(GetMoneyExchangeResponse).GetProperties();
                     string strRegex = @"(^[a-zA-Z]*_(ASC|asc)$)|(^[a-zA-Z]*_(DESC|desc))";
                     Regex regex = new Regex(strRegex);
                     if (sortBy is not null)
                     {
                         if (regex.IsMatch(sortBy.Trim()) == false)
                         {
                             context.AddFailure("SortBy", "Sort by is required following format: propertyName_ASC | propertyName_DESC.");
                         }
                         string[] sortByParts = sortBy.Split("_");
                         if (properties.Any(x => x.Name.ToLower().Equals(sortByParts[0].Trim().ToLower())) == false)
                         {
                             context.AddFailure("SortBy", "Property name in format does not exist in the system.");
                         }
                     }
                 });

            #region Status
            RuleFor(x => x.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .Must((x, status) =>
                     {
                         if (status == null)
                         {
                             return true; // Skip validation when status is null
                         }
                         return StringUtil.CheckMoneyExchangeStatus(status);
                     }).WithMessage("{PropertyName} is required SUCCESS or FAIL");
            #endregion

            #region SearchDateFrom
            RuleFor(x => x.SearchDateFrom)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .Matches(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[012])\/(19|20|)\d\d$")
                     .WithMessage("{PropertyName} is required dd/MM/yyyy");
            #endregion

            #region SearchDateFrom
            RuleFor(x => x.SearchDateTo)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .Matches(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[012])\/(19|20|)\d\d$")
                     .WithMessage("{PropertyName} is required dd/MM/yyyy");
            #endregion
        }
    }
}
