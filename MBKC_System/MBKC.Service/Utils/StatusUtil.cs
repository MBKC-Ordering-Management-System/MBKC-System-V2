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
            } else if (status == (int)StoreEnum.Status.BE_CONFIRMING)
            {
                return char.ToUpper(StoreEnum.Status.BE_CONFIRMING.ToString()[0]) + StoreEnum.Status.BE_CONFIRMING.ToString().ToLower().Substring(1);
            }else if (status == (int)StoreEnum.Status.REJECTED)
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
    }
}
