using MBKC.Repository.Enums;
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
            string[] statusNameParts = StoreEnum.Status.BE_CONFIRMING.ToString().Split("_");
            if (statusName.ToLower().Trim().Equals(StoreEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(StoreEnum.Status.INACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals(StoreEnum.Status.REJECTED.ToString().ToLower()) ||
                statusName.ToLower().Trim().Equals($"{statusNameParts[0]} {statusNameParts[1]}".ToLower()))
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
            if(productType.Trim().ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
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
            return $"in the amount of: {amount}đ at {DateTime.Now.Hour}:{DateTime.Now.Minute} - {DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year}";
        }

        public static string ConvertTimeToCron(int hour, int minute)
        {
            return $"{minute} {hour} * * *";
        }

        public static (int hour, int minute) ConvertCronToTime(string cron)
        {
            var cronFields = Regex.Split(cron, @"\s+");
            int hour = int.Parse(cronFields[1]);
            int minutes = int.Parse(cronFields[0]);
            return (hour, minutes);
        }
    }
}
