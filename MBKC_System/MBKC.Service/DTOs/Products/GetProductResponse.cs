using MBKC.Repository.Models;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.DTOs.Categories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Products
{
    public class GetProductResponse
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal HistoricalPrice { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public string? Size { get; set; }
        public int DisplayOrder { get; set; }
        public string? ParentProductId { get; set; }
        public IEnumerable<GetProductResponse>? ChildrenProducts { get; set; }
        public int CategoryId { get; set; }
        public GetBrandResponse Brand { get; set; }
    }
}
