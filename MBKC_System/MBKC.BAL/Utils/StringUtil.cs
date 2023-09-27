using MBKC.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Utils
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
            if (statusName.ToLower().Equals(KitchenCenterEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Equals(KitchenCenterEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckStoreStatusName(string statusName)
        {
            if (statusName.ToLower().Equals(StoreEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Equals(StoreEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool  IsUnicode(string input)
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
            return stringBuilder.ToString();
        }

        public static bool CheckCategoryStatusName(string statusName)
        {
            if (statusName.ToLower().Equals(CategoryEnum.Status.ACTIVE.ToString().ToLower()) ||
                statusName.ToLower().Equals(CategoryEnum.Status.INACTIVE.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool CheckCategoryType(string type)
        {
            if (type.ToLower().Equals(CategoryEnum.Type.NORMAL.ToString().ToLower()) ||
                type.ToLower().Equals(CategoryEnum.Type.EXTRA.ToString().ToLower()))
            {
                return true;
            }
            return false;
        }
    }
}
