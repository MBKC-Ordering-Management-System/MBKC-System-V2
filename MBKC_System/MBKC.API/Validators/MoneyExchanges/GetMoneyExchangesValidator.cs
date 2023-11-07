using FluentValidation;
using MBKC.Service.DTOs.MoneyExchanges;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.Utils;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MBKC.API.Validators.MoneyExchanges
{
    public class GetMoneyExchangesValidator : AbstractValidator<GetMoneyExchangeRequest>
    {
        public GetMoneyExchangesValidator()
        {
            RuleFor(x => x.CurrentPage)
              .Cascade(CascadeMode.Stop)
              .Custom((currentPage, context) =>
              {
                  if (currentPage <= 0)
                  {
                      context.AddFailure("CurrentPage", "Current page number is required more than 0.");
                  }
              });

            RuleFor(x => x.ItemsPerPage)
                .Cascade(CascadeMode.Stop)
                .Custom((itemsPerPage, context) =>
                {
                    if (itemsPerPage <= 0)
                    {
                        context.AddFailure("ItemsPerPage", "Items per page number is required more than 0.");
                    }
                });

            RuleFor(x => x.SortBy)
                 .Cascade(CascadeMode.Stop)
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

            RuleFor(x => x.ExchangeType)
                       .Cascade(CascadeMode.Stop)
                       .Must((_, type) =>
                       {
                           if (type == null)
                           {
                               return true; 
                           }
                           return StringUtil.CheckMoneyExchangeType(type);
                       }).WithMessage("{PropertyName} is required SEND, RECEIVE, WITHDRAW.");
        }
    }
}
