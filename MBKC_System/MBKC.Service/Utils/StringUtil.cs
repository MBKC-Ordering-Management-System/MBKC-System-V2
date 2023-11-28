using MBKC.Repository.Enums;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.MoneyExchanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MBKC.Service.Utils
{
    public static class StringUtil
    {
        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };

        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }

        public static bool CheckUrlString(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        public static bool CheckKitchenCenterStatusName(string statusName)
        {
            if (statusName.ToLower().Trim().Equals(KitchenCenterEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(KitchenCenterEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckStoreStatusName(string statusName)
        {
            if (statusName.ToLower().Trim().Equals(StoreEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(StoreEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckStoreStatusNameParam(string statusName)
        {
            if (statusName.ToLower().Trim().Equals(StoreEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(StoreEnum.Status.INACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(StoreEnum.Status.REJECTED.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(StoreEnum.Status.CONFIRMING.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool IsUnicode(string input)
        {
            var asciiBytesCount = Encoding.ASCII.GetByteCount(input);
            var unicodBytesCount = Encoding.UTF8.GetByteCount(input);
            return asciiBytesCount != unicodBytesCount;
        }

        public static string EncryptData(string data)
        {
            MD5 mD5 = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            byte[] hash = mD5.ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }
            return stringBuilder.ToString().ToLower();
        }

        public static bool CheckCategoryStatusName(string statusName)
        {
            if (statusName.ToLower().Trim().Equals(CategoryEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(CategoryEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckCategoryType(string type)
        {
            if (type.ToLower().Trim().Equals(CategoryEnum.Type.NORMAL.ToString().ToLower()) ||
                type.ToLower().Trim().Equals(CategoryEnum.Type.EXTRA.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckBrandStatusName(string statusName)
        {
            if (statusName.ToLower().Trim().Equals(BrandEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(BrandEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckConfirmStoreRegistrationStatusName(string statusName)
        {
            if (statusName.ToLower().Trim().Equals(StoreEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(StoreEnum.Status.REJECTED.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckBankingAccountStatusName(string statusName)
        {
            if (statusName.ToLower().Trim().Equals(BankingAccountEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(BankingAccountEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool IsDigitString(string value)
        {
            return value.All(char.IsDigit);
        }

        public static bool CheckProductType(string productType)
        {
            if (productType.Trim().ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                productType.Trim().ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()) ||
                productType.Trim().ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()) ||
                productType.Trim().ToLower().Equals(ProductEnum.Type.EXTRA.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckPartnerStatusName(string statusName)
        {
            if (statusName.ToLower().Trim().Equals(PartnerEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(PartnerEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckPartnerProductStatusName(string statusName)
        {
            if (statusName.ToLower().Trim().Equals(PartnerProductEnum.Status.AVAILABLE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(PartnerProductEnum.Status.OUT_OF_STOCK_TODAY.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(PartnerProductEnum.Status.OUT_OF_STOCK_INDENTIFINITELY.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool IsMD5(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, @"^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }

        public static bool CheckStorePartnerStatusName(string statusName)
        {
            if (statusName.ToLower().Trim().Equals(StorePartnerEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(StorePartnerEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static string GetContentAmountAndTime(decimal amount)
        {
            return $"in the amount of: {(int)Math.Floor(amount)}đ at {DateTime.Now.Hour}:{DateTime.Now.Minute} - {DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year}";
        }

        public static string GetContentRejectReason()
        {
            return $"because it has not been processed on the day at {DateTime.Now.Hour}:{DateTime.Now.Minute} - {DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year}";
        }

        public static string ConvertTimeSpanToCron(TimeSpan timeSpan)
        {
            string minute = timeSpan.Minutes == 0 ? "*" : timeSpan.Minutes.ToString();

            return $"{minute} {timeSpan.Hours} * * *";
        }

        public static string GetStringErrorWithProductType(string productType)
        {
            if (productType.ToUpper().Equals(ProductEnum.Type.PARENT.ToString()))
            {
                return MessageConstant.ProductMessage.InvalidProductTypeParent;
            }
            else if (productType.ToUpper().Equals(ProductEnum.Type.CHILD.ToString()))
            {
                return MessageConstant.ProductMessage.InvalidProductTypeChild;
            }
            else if (productType.ToUpper().Equals(ProductEnum.Type.SINGLE.ToString()))
            {
                return MessageConstant.ProductMessage.InvalidProductTypeSingle;
            }

            return MessageConstant.ProductMessage.InvalidProductTypeExtra;
        }

        public static bool CheckSystemStatusOrder(string status)
        {
            if (status.Trim().ToLower().Equals(OrderEnum.SystemStatus.IN_STORE.ToString().ToLower()) ||
                status.Trim().ToLower().Equals(OrderEnum.SystemStatus.READY_DELIVERY.ToString().ToLower()) ||
                status.Trim().ToLower().Equals(OrderEnum.SystemStatus.COMPLETED.ToString().ToLower()) ||
                status.Trim().ToLower().Equals(OrderEnum.SystemStatus.CANCELLED.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckMoneyExchangeType(string type)
        {
            if (type.Trim().ToUpper().Equals(MoneyExchangeEnum.ExchangeType.SEND.ToString()) ||
                type.Trim().ToUpper().Equals(MoneyExchangeEnum.ExchangeType.RECEIVE.ToString()) ||
                type.Trim().ToUpper().Equals(MoneyExchangeEnum.ExchangeType.WITHDRAW.ToString()))
            {
                return true;
            }
            return false;
        }


        public static bool CheckParnerOrderStatus(string status)
        {
            if (status.Trim().ToLower().Equals(OrderEnum.Status.READY.ToString().ToLower()) ||
                status.Trim().ToLower().Equals(OrderEnum.Status.PREPARING.ToString().ToLower()) ||
                status.Trim().ToLower().Equals(OrderEnum.Status.UPCOMING.ToString().ToLower()) ||
                status.Trim().ToLower().Equals(OrderEnum.Status.COMPLETED.ToString().ToLower()) ||
                status.Trim().ToLower().Equals(OrderEnum.Status.CANCELLED.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckMoneyExchangeStatus(string status)
        {
            if (status.Trim().ToLower().Equals(MoneyExchangeEnum.Status.SUCCESS.ToString().ToLower()) ||
                status.Trim().ToLower().Equals(MoneyExchangeEnum.Status.FAIL.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckExchangeType(string exchangeType)
        {
            if (exchangeType.Trim().ToLower().Equals(MoneyExchangeEnum.ExchangeType.SEND.ToString().ToLower()) ||
                exchangeType.Trim().ToLower().Equals(MoneyExchangeEnum.ExchangeType.RECEIVE.ToString().ToLower()) ||
                exchangeType.Trim().ToLower().Equals(MoneyExchangeEnum.ExchangeType.WITHDRAW.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckPaymentMethod(string paymentMethod)
        {
            if (paymentMethod.Trim().ToLower().Equals(ShipperPaymentEnum.PaymentMethod.CASH.ToString().ToLower()) ||
                paymentMethod.Trim().ToLower().Equals(ShipperPaymentEnum.PaymentMethod.CASHLESS.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckShipperPaymentStatus(string status)
        {
            if (status.Trim().ToLower().Equals(ShipperPaymentEnum.Status.SUCCESS.ToString().ToLower()) ||
                status.Trim().ToLower().Equals(ShipperPaymentEnum.Status.FAIL.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }
    }
}
