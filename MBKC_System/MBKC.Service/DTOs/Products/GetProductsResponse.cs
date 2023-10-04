using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Products
{
    public class GetProductsResponse
    {
        public int TotalPages { get; set; }
        public int NumberItems { get; set; }
        public List<GetProductResponse> Products { get; set; }
    }
}
