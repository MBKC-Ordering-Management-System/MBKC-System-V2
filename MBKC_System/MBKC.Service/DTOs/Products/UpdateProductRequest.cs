using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Products
{
    public class UpdateProductRequest
    {
        public string? Name { get; set; }
        public string Description { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? HistoricalPrice { get; set; }
        public int DisplayOrder { get; set; }
        public int? ParentProductId { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile? Image { get; set; }
        public string Status { get; set; }
    }
}
