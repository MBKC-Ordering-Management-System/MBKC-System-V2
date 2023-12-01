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
            return char.ToUpper(BrandEnum.Status.DISABLE.ToString()[0]) + BrandEnum.Status.DISABLE.ToString().ToLower().Substring(1);
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
            return char.ToUpper(KitchenCenterEnum.Status.DISABLE.ToString()[0]) + KitchenCenterEnum.Status.DISABLE.ToString().ToLower().Substring(1);
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
            else if (status == (int)StoreEnum.Status.CONFIRMING)
            {
                return char.ToUpper(StoreEnum.Status.CONFIRMING.ToString()[0]) + StoreEnum.Status.CONFIRMING.ToString().ToLower().Substring(1);
            }
            else if (status == (int)StoreEnum.Status.REJECTED)
            {
                return char.ToUpper(StoreEnum.Status.REJECTED.ToString()[0]) + StoreEnum.Status.REJECTED.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(StoreEnum.Status.DISABLE.ToString()[0]) + StoreEnum.Status.DISABLE.ToString().ToLower().Substring(1);
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
            return char.ToUpper(CategoryEnum.Status.DISABLE.ToString()[0]) + CategoryEnum.Status.DISABLE.ToString().ToLower().Substring(1);
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
            return char.ToUpper(BankingAccountEnum.Status.DISABLE.ToString()[0]) + BankingAccountEnum.Status.DISABLE.ToString().ToLower().Substring(1);
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
            return char.ToUpper(ProductEnum.Status.DISABLE.ToString()[0]) + ProductEnum.Status.DISABLE.ToString().ToLower().Substring(1);
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
            return char.ToUpper(PartnerEnum.Status.DISABLE.ToString()[0]) + PartnerEnum.Status.DISABLE.ToString().ToLower().Substring(1);

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
            return char.ToUpper(CashierEnum.Status.DISABLE.ToString()[0]) + CashierEnum.Status.DISABLE.ToString().ToLower().Substring(1);

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
            return char.ToUpper(AccountEnum.Status.DISABLE.ToString()[0]) + AccountEnum.Status.DISABLE.ToString().ToLower().Substring(1);

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
            return char.ToUpper(StorePartnerEnum.Status.DISABLE.ToString()[0]) + StorePartnerEnum.Status.DISABLE.ToString().ToLower().Substring(1);

        }

        public static string ChangePartnerProductStatus(int status)
        {
            if (status == (int)PartnerProductEnum.Status.AVAILABLE)
            {
                return char.ToUpper(PartnerProductEnum.Status.AVAILABLE.ToString()[0]) + PartnerProductEnum.Status.AVAILABLE.ToString().ToLower().Substring(1);
            }
            else if (status == (int)PartnerProductEnum.Status.OUT_OF_STOCK_TODAY)
            {
                return "Out of stock today";
            }
            else if (status == (int)PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY)
            {
                return "Out of stock Indentifinitely";
            }
            return char.ToUpper(PartnerProductEnum.Status.DISABLE.ToString()[0]) + PartnerProductEnum.Status.DISABLE.ToString().ToLower().Substring(1);
        }

        public static int? ChangeStoreStatus(string status)
        {
            string[] statusNameParts = StoreEnum.Status.CONFIRMING.ToString().Split("_");
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
            else if (status.ToLower().Equals(StoreEnum.Status.CONFIRMING.ToString().ToLower()))
            {
                return (int)StoreEnum.Status.CONFIRMING;
            }
            return null;
        }

        public static string ChangeTransactionStatus(int status)
        {
            if (status == (int)TransactionEnum.Status.FAIL)
            {
                return char.ToUpper(TransactionEnum.Status.FAIL.ToString()[0]) + TransactionEnum.Status.FAIL.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(TransactionEnum.Status.SUCCESS.ToString()[0]) + TransactionEnum.Status.SUCCESS.ToString().ToLower().Substring(1);
        }

        public static string ChangeShipperPaymentStatus(int status)
        {
            if (status == (int)ShipperPaymentEnum.Status.SUCCESS)
            {
                return char.ToUpper(ShipperPaymentEnum.Status.SUCCESS.ToString()[0]) + ShipperPaymentEnum.Status.SUCCESS.ToString().ToLower().Substring(1);
            }

            return char.ToUpper(ShipperPaymentEnum.Status.FAIL.ToString()[0]) + ShipperPaymentEnum.Status.FAIL.ToString().ToLower().Substring(1);
        }

        public static string ChangeMoneyExchangeStatus(int status)
        {
            if (status == (int)MoneyExchangeEnum.Status.SUCCESS)
            {
                return char.ToUpper(MoneyExchangeEnum.Status.SUCCESS.ToString()[0]) + MoneyExchangeEnum.Status.SUCCESS.ToString().ToLower().Substring(1);
            }

            return char.ToUpper(MoneyExchangeEnum.Status.FAIL.ToString()[0]) + MoneyExchangeEnum.Status.FAIL.ToString().ToLower().Substring(1);
        }

        public static string ChangePartnerOrderStatus(string status)
        {
            if (status.ToUpper().Equals(OrderEnum.Status.READY.ToString()))
            {
                return char.ToUpper(OrderEnum.Status.READY.ToString()[0]) + OrderEnum.Status.READY.ToString().ToLower().Substring(1);
            }
            else if (status.ToUpper().Equals(OrderEnum.Status.PREPARING.ToString()))
            {
                return char.ToUpper(OrderEnum.Status.PREPARING.ToString()[0]) + OrderEnum.Status.PREPARING.ToString().ToLower().Substring(1);
            }
            else if (status.ToUpper().Equals(OrderEnum.Status.CANCELLED.ToString()))
            {
                return char.ToUpper(OrderEnum.Status.CANCELLED.ToString()[0]) + OrderEnum.Status.CANCELLED.ToString().ToLower().Substring(1);
            }
            else if (status.ToUpper().Equals(OrderEnum.Status.COMPLETED.ToString()))
            {
                return char.ToUpper(OrderEnum.Status.COMPLETED.ToString()[0]) + OrderEnum.Status.COMPLETED.ToString().ToLower().Substring(1);
            }
            return "Upcoming";
        }

        public static string ChangeSystemOrderStatus(string status)
        {
            if (status.ToUpper().Equals(OrderEnum.SystemStatus.READY_DELIVERY.ToString()))
            {
                return "Ready delivery";
            }
            else if (status.ToUpper().Equals(OrderEnum.SystemStatus.IN_STORE.ToString()))
            {
                return "In store";
            }
            else if (status.ToUpper().Equals(OrderEnum.SystemStatus.CANCELLED.ToString()))
            {
                return char.ToUpper(OrderEnum.SystemStatus.CANCELLED.ToString()[0]) + OrderEnum.SystemStatus.CANCELLED.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(OrderEnum.SystemStatus.COMPLETED.ToString()[0]) + OrderEnum.SystemStatus.COMPLETED.ToString().ToLower().Substring(1);
        }

        public static int ChangeMoneyExchangeStatus(string status)
        {
            if (status.ToUpper().Equals(MoneyExchangeEnum.Status.SUCCESS.ToString()))
            {
                return (int)MoneyExchangeEnum.Status.SUCCESS;
            }
            return (int)MoneyExchangeEnum.Status.FAIL;
        }

        public static int ChangeShipperPaymentStatus(string status)
        {
            if (status.ToUpper().Equals(ShipperPaymentEnum.Status.SUCCESS.ToString()))
            {
                return (int)ShipperPaymentEnum.Status.SUCCESS;
            }
            return (int)ShipperPaymentEnum.Status.FAIL;
        }

        public static string ChangeMoneyExchangeType(string status)
        {
            if (status.ToUpper().Equals(MoneyExchangeEnum.ExchangeType.SEND.ToString()))
            {
                return char.ToUpper(MoneyExchangeEnum.ExchangeType.SEND.ToString()[0]) + MoneyExchangeEnum.ExchangeType.SEND.ToString().ToLower().Substring(1);
            }
            else if (status.ToUpper().Equals(MoneyExchangeEnum.ExchangeType.RECEIVE.ToString()))
            {
                return char.ToUpper(MoneyExchangeEnum.ExchangeType.RECEIVE.ToString()[0]) + MoneyExchangeEnum.ExchangeType.RECEIVE.ToString().ToLower().Substring(1);
            }
            return char.ToUpper(MoneyExchangeEnum.ExchangeType.WITHDRAW.ToString()[0]) + MoneyExchangeEnum.ExchangeType.WITHDRAW.ToString().ToLower().Substring(1);
        }
    }
}

