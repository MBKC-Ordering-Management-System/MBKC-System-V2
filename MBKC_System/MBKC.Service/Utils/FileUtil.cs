using Microsoft.AspNetCore.Http;
using System.Web;

namespace MBKC.Service.Utils
{
    public static class FileUtil
    {

        #region Convert IFormFile to FileStream
        public static FileStream ConvertFormFileToStream(IFormFile file)
        {
            // Create a unique temporary file path
            string tempFilePath = Path.GetTempFileName();

            // Open a FileStream to write the file
            using (FileStream stream = new FileStream(tempFilePath, FileMode.Create))
            {
                // Copy the contents of the IFormFile to the FileStream
                file.CopyToAsync(stream).GetAwaiter().GetResult();
            }

            // Open the temporary file in read mode and return the FileStream
            FileStream fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);

            return fileStream;
        }
        #endregion

        #region HaveSupportedFileType
        public static bool HaveSupportedFileType(string fileName)
        {
            string[] validFileTypes = { ".png", ".jpg", ".jpeg" , ".webp" };
            string extensionFile = Path.GetExtension(fileName);
            if (validFileTypes.Contains(extensionFile))
            {
                return true;
            }
            return false;
        }
        #endregion

        public static string GetImageIdFromUrlImage(string urlImage, string queryName)
        {
            try
            {
                Uri imageUri = new Uri(urlImage);

                string imageId = HttpUtility.ParseQueryString(imageUri.Query).Get(queryName);

                return imageId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
