using Microsoft.AspNetCore.Http;
using System.Web;
using Spire.Xls;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Products;
using MBKC.Service.Exceptions;
using Microsoft.Office.Interop.Excel;
using Workbook = Spire.Xls.Workbook;
using Worksheet = Spire.Xls.Worksheet;

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

        #region Convert Excel Picture to FileStream
        public static FileStream ConvertExcelPictureToStream(ExcelPicture file)
        {
            // Create a unique temporary file path
            string tempFilePath = Path.GetTempFileName();

            // Open a FileStream to write the file
            using (FileStream stream = new FileStream(tempFilePath, FileMode.Create))
            {
                // Copy the contents of the IFormFile to the FileStream
                file.SaveToImage(stream);
            }

            // Open the temporary file in read mode and return the FileStream
            FileStream fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);

            return fileStream;
        }
        #endregion

        #region get data from excel file
        public static List<CreateProductExcelRequest> GetDataFromExcelFile(IFormFile excelFile)
        {
            try
            {
                List<CreateProductExcelRequest> products = new List<CreateProductExcelRequest>();

                using (var stream = new MemoryStream())
                {
                    excelFile.CopyTo(stream);
                    Workbook workbook = new Workbook();
                    workbook.LoadFromStream(stream);
                    Worksheet worksheet = workbook.Worksheets[0];
                    int rowCount = worksheet.Rows.Length;

                    if (rowCount == 0)
                    {
                        throw new BadRequestException(MessageConstant.ProductMessage.ExcelFileHasNoData);
                    }

                    // start at row 2
                    for (int row = 2; row <= rowCount; row++)
                    {
                        CreateProductExcelRequest product = new CreateProductExcelRequest
                        {
                            row = row,
                            Code = string.IsNullOrEmpty(worksheet.Range[row, 1].Value.ToString()) ? null : worksheet.Range[row, 1].Value.ToString(),
                            Name = string.IsNullOrEmpty(worksheet.Range[row, 2].Value.ToString()) ? null : worksheet.Range[row, 2].Value.ToString(),
                            Description = string.IsNullOrEmpty(worksheet.Range[row, 3].Value.ToString()) ? null : worksheet.Range[row, 3].Value.ToString(),
                            SellingPrice = string.IsNullOrEmpty(worksheet.Range[row, 4].Value.ToString()) ? 0 : decimal.Parse(worksheet.Range[row, 4].Value.ToString()),
                            DiscountPrice = string.IsNullOrEmpty(worksheet.Range[row, 5].Value.ToString()) ? 0 : decimal.Parse(worksheet.Range[row, 5].Value.ToString()),
                            HistoricalPrice = string.IsNullOrEmpty(worksheet.Range[row, 6].Value.ToString()) ? 0 : decimal.Parse(worksheet.Range[row, 6].Value.ToString()),
                            Size = string.IsNullOrEmpty(worksheet.Range[row, 7].Value.ToString()) ? null : worksheet.Range[row, 7].Value.ToString(),
                            Type = string.IsNullOrEmpty(worksheet.Range[row, 8].Value.ToString()) ? null : worksheet.Range[row, 8].Value.ToString(),
                            Image = worksheet.Pictures[row - 2] == null ? null : ConvertExcelPictureToStream(worksheet.Pictures[row - 2]),
                            DisplayOrder = string.IsNullOrEmpty(worksheet.Range[row, 10].Value.ToString()) ? 0 : int.Parse(worksheet.Range[row, 10].Value.ToString()),
                            ParentProductId = string.IsNullOrEmpty(worksheet.Range[row, 11].Value.ToString()) ? null : int.Parse(worksheet.Range[row, 11].Value.ToString()),
                            CategoryId = string.IsNullOrEmpty(worksheet.Range[row, 12].Value.ToString()) ? null : int.Parse(worksheet.Range[row, 12].Value.ToString()),
                        };
                        products.Add(product);
                    }
                }

                return products;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region HaveSupportedFileType
        public static bool HaveSupportedFileType(string fileName)
        {
            string[] validFileTypes = { ".png", ".jpg", ".jpeg", ".webp" };
            string extensionFile = Path.GetExtension(fileName);
            if (validFileTypes.Contains(extensionFile))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region HaveSupportedFileTypeExcel
        public static bool HaveSupportedFileTypeExcel(string fileName)
        {
            string[] validFileTypes = { ".xlsx" };
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
