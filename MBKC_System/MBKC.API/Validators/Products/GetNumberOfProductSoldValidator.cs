using FluentValidation;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Products;
using MBKC.Service.Utils;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MBKC.API.Validators.Products
{
    public class GetNumberOfProductSoldValidator : AbstractValidator<GetProductWithNumberSoldRequest>
    {
        public GetNumberOfProductSoldValidator()
        {
            RuleFor(p => p.ProductSearchDateFrom)
                    .Cascade(CascadeMode.Stop)
                    .Matches(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[012])\/(19|20|)\d\d$")
                    .WithMessage("{PropertyName} is required dd/MM/yyyy");

            RuleFor(p => p.ProductSearchDateTo)
                     .Cascade(CascadeMode.Stop)
                     .Matches(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[012])\/(19|20|)\d\d$")
                     .WithMessage("{PropertyName} is required dd/MM/yyyy");

            RuleFor(p => p)
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

            RuleFor(p => p.CurrentPage)
                          .Cascade(CascadeMode.Stop)
                          .Custom((currentPage, context) =>
                          {
                              if (currentPage <= 0)
                              {
                                  context.AddFailure("CurrentPage", "Current page number is required more than 0.");
                              }
                          });

            RuleFor(p => p.ItemsPerPage)
                .Cascade(CascadeMode.Stop)
                .Custom((itemsPerPage, context) =>
                {
                    if (itemsPerPage <= 0)
                    {
                        context.AddFailure("ItemsPerPage", "Items per page number is required more than 0.");
                    }
                });

            RuleFor(p => p.SortBy)
                .Cascade(CascadeMode.Stop)
                .Custom((sortBy, context) =>
                {
                    PropertyInfo[] properties = typeof(GetProductResponse).GetProperties();
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

            RuleFor(p => p.IdCategory)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

            RuleFor(p => p)
           .Cascade(CascadeMode.Stop)
           .Custom((product, context) =>
           {
               if (string.IsNullOrWhiteSpace(product.ProductType) == false)
               {
                   if (!product.ProductType.ToUpper().Equals(ProductEnum.Type.SINGLE.ToString()) && !product.ProductType.ToUpper().Equals(ProductEnum.Type.CHILD.ToString()))
                   {
                       context.AddFailure("ProductType", "The product type must be SINGLE or CHILD.");
                   }
               }
           });
        }
    }
}
