using MBKC.Service.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Categories
{
    public class GetExtraCategoryResponse
    {
        public int CategoryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int DisplayOrder { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; }
        public List<GetProductResponse> ExtraProducts { get; set; }
    }
}
