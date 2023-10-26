using MBKC.Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Utils
{
    public static class StatusUtil
    {
        public static string ChangeBrandStatus(int status)
        {
            if (status == (int)BrandEnum.Status.INACTIVE)
            {
                return char.ToUpper(BrandEnum.Status.INACTIVE.ToString()[0]) + BrandEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)BrandEnum.Status.ACTIVE)
            {
                return char.ToUpper(BrandEnum.Status.ACTIVE.ToString()[0]) + BrandEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(BrandEnum.Status.DEACTIVE.ToString()[0]) + BrandEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);
        }

        public static string ChangeKitchenCenterStatus(int status)
        {
            if (status == (int)KitchenCenterEnum.Status.INACTIVE)
            {
                return char.ToUpper(KitchenCenterEnum.Status.INACTIVE.ToString()[0]) + KitchenCenterEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)KitchenCenterEnum.Status.ACTIVE)
            {
                return char.ToUpper(KitchenCenterEnum.Status.ACTIVE.ToString()[0]) + KitchenCenterEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(KitchenCenterEnum.Status.DEACTIVE.ToString()[0]) + KitchenCenterEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);
        }

        public static string ChangeStoreStatus(int status)
        {
            if (status == (int)StoreEnum.Status.INACTIVE)
            {
                return char.ToUpper(StoreEnum.Status.INACTIVE.ToString()[0]) + StoreEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)StoreEnum.Status.ACTIVE)
            {
                return char.ToUpper(StoreEnum.Status.ACTIVE.ToString()[0]) + StoreEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)StoreEnum.Status.BE_CONFIRMING)
            {
                string[] statusNameParts = StoreEnum.Status.BE_CONFIRMING.ToString().Split("_");
                return char.ToUpper(statusNameParts[0][0]) + statusNameParts[0].ToLower().Substring(1) + " " + char.ToUpper(statusNameParts[1][0]) + statusNameParts[1].ToLower().Substring(1);
            }
            else if (status == (int)StoreEnum.Status.REJECTED)
            {
                return char.ToUpper(StoreEnum.Status.REJECTED.ToString()[0]) + StoreEnum.Status.REJECTED.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(StoreEnum.Status.DEACTIVE.ToString()[0]) + StoreEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);
        }

        public static string ChangeCategoryStatus(int status)
        {
            if (status == (int)CategoryEnum.Status.INACTIVE)
            {
                return char.ToUpper(CategoryEnum.Status.INACTIVE.ToString()[0]) + CategoryEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)CategoryEnum.Status.ACTIVE)
            {
                return char.ToUpper(StoreEnum.Status.ACTIVE.ToString()[0]) + CategoryEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(CategoryEnum.Status.DEACTIVE.ToString()[0]) + CategoryEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);
        }

        public static string ChangeBankingAccountStatus(int status)
        {
            if (status == (int)BankingAccountEnum.Status.INACTIVE)
            {
                return char.ToUpper(BankingAccountEnum.Status.INACTIVE.ToString()[0]) + BankingAccountEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)BankingAccountEnum.Status.ACTIVE)
            {
                return char.ToUpper(BankingAccountEnum.Status.ACTIVE.ToString()[0]) + BankingAccountEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(BankingAccountEnum.Status.DEACTIVE.ToString()[0]) + BankingAccountEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);
        }

        public static string ChangeProductStatusStatus(int status)
        {
            if (status == (int)ProductEnum.Status.INACTIVE)
            {
                return char.ToUpper(ProductEnum.Status.INACTIVE.ToString()[0]) + ProductEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)ProductEnum.Status.ACTIVE)
            {
                return char.ToUpper(ProductEnum.Status.ACTIVE.ToString()[0]) + ProductEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(ProductEnum.Status.DEACTIVE.ToString()[0]) + ProductEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);
        }

        public static string ChangePartnerStatus(int status)
        {
            if (status == (int)PartnerEnum.Status.INACTIVE)
            {
                return char.ToUpper(PartnerEnum.Status.INACTIVE.ToString()[0]) + PartnerEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)PartnerEnum.Status.ACTIVE)
            {
                return char.ToUpper(PartnerEnum.Status.ACTIVE.ToString()[0]) + PartnerEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(PartnerEnum.Status.DEACTIVE.ToString()[0]) + PartnerEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);

        }


        public static string ChangeCashierStatus(int status)
        {
            if (status == (int)CashierEnum.Status.INACTIVE)
            {
                return char.ToUpper(CashierEnum.Status.INACTIVE.ToString()[0]) + CashierEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)CashierEnum.Status.ACTIVE)
            {
                return char.ToUpper(CashierEnum.Status.ACTIVE.ToString()[0]) + CashierEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(CashierEnum.Status.DEACTIVE.ToString()[0]) + CashierEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);

        }


        public static string ChangeAccountStatus(int status)
        {
            if (status == (int)AccountEnum.Status.INACTIVE)
            {
                return char.ToUpper(AccountEnum.Status.INACTIVE.ToString()[0]) + AccountEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)AccountEnum.Status.ACTIVE)
            {
                return char.ToUpper(AccountEnum.Status.ACTIVE.ToString()[0]) + AccountEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(AccountEnum.Status.DEACTIVE.ToString()[0]) + AccountEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);

        }

        public static string ChangeStorePartnerStatus(int status)
        {
            if (status == (int)StorePartnerEnum.Status.INACTIVE)
            {
                return char.ToUpper(StorePartnerEnum.Status.INACTIVE.ToString()[0]) + StorePartnerEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)StorePartnerEnum.Status.ACTIVE)
            {
                return char.ToUpper(StorePartnerEnum.Status.ACTIVE.ToString()[0]) + StorePartnerEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(StorePartnerEnum.Status.DEACTIVE.ToString()[0]) + StorePartnerEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);

        }

        public static string ChangePartnerProductStatus(int status)
        {
            if (status == (int)GrabFoodItemEnum.AvailableStatus.AVAILABLE)
            {
                return char.ToUpper(GrabFoodItemEnum.AvailableStatus.AVAILABLE.ToString()[0]) + GrabFoodItemEnum.AvailableStatus.AVAILABLE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)GrabFoodItemEnum.AvailableStatus.OUT_OF_STOCK_TODAY)
            {
                return "Out of stock today";
            }
            else if (status == (int)GrabFoodItemEnum.AvailableStatus.OUT_OF_STOCK_INDENTIFINITELY)
            {
                return "Out of stock Indentifinitely";
            }
            return char.ToUpper(GrabFoodItemEnum.AvailableStatus.DEACTIVE.ToString()[0]) + GrabFoodItemEnum.AvailableStatus.DEACTIVE.ToString().ToLower().Substring(1);
        }

        public static int? ChangeStoreStatus(string status)
        {
            string[] statusNameParts = StoreEnum.Status.BE_CONFIRMING.ToString().Split("_");
            if (status.ToLower().Equals(StoreEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return (int)StoreEnum.Status.INACTIVE;
            }
            else if (status.ToLower().Equals(StoreEnum.Status.ACTIVE.ToString().ToLower()))
            {
                return (int)StoreEnum.Status.ACTIVE;
            }
            else if (status.ToLower().Equals(StoreEnum.Status.REJECTED.ToString().ToLower()))
            {
                return (int)StoreEnum.Status.REJECTED;
            }
            else if (status.ToLower().Equals($"{statusNameParts[0]} {statusNameParts[1]}".ToLower()))
            {
                return (int)StoreEnum.Status.BE_CONFIRMING;
            }
            return null;
        }
    }
}

