using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Utils
{
    public static class SplitStringUtil
    {
        public static string SplitString(string url, string folder)
        {
            // Tìm vị trí của chuỗi "KitchenCenter%2F"
            int folderIndex = url.IndexOf($"{folder}%2F");

            // Cắt bỏ phần đầu tiên
            string remainingUrl = url.Substring(folderIndex + $"{folder}%2F".Length);

            // Tìm vị trí của dấu chấm hết thư mục
            int dotIndex = remainingUrl.IndexOf("?");

            // Cắt bỏ phần sau dấu chấm hết thư mục
            string fileId = remainingUrl.Substring(0, dotIndex);
            return fileId;
        }
    }
}
