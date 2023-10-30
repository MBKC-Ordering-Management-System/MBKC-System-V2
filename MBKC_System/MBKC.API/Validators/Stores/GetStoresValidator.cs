using FluentValidation;
using MBKC.Repository.Utils;
using MBKC.Service.DTOs.Stores;
using System.Reflection;
using System.Text.RegularExpressions;
using MBKC.Service.Utils;
using MBKC.Service.Constants;

namespace MBKC.API.Validators.Stores
{
    public class GetStoresValidator : AbstractValidator<GetStoresRequest>
    {
        public GetStoresValidator()
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
                    PropertyInfo[] properties = typeof(GetStoreResponse).GetProperties();
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

            RuleFor(x => x.IdBrand)
               .Cascade(CascadeMode.StopOnFirstFailure)
               .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

            RuleFor(x => x.IdKitchenCenter)
               .Cascade(CascadeMode.StopOnFirstFailure)
               .GreaterThan(0).WithMessage("{PropertyName} is not suitable id in the system.");

            /*RuleFor(x => x.Status)
                     .Cascade(CascadeMode.StopOnFirstFailure)
                     .Must(Service.Utils.StringUtil.CheckStoreStatusNameParam).WithMessage(MessageConstant.StoreMessage.StoresWithStatusNameParam);*/

            #region KeySortStatus
            RuleFor(x => x.Status)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .Must((x, status) =>
                        {
                            if (status == null)
                            {
                                return true; // Skip validation when keySortName is null
                            }
                            return Service.Utils.StringUtil.CheckStoreStatusNameParam(status);
                        }).WithMessage(MessageConstant.StoreMessage.StoresWithStatusNameParam);
            #endregion
        }

    }
}
