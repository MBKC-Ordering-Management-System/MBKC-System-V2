using Microsoft.AspNetCore.Http;

namespace MBKC.Service.DTOs.Products
{
    public class CreateProductRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? HistoricalPrice { get; set; }
        public string? Size { get; set; }
        public string Type { get; set; }
        public IFormFile Image { get; set; }
        public int DisplayOrder { get; set; }
        public int? ParentProductId { get; set; }
        public int? CategoryId { get; set; }
    }
}
