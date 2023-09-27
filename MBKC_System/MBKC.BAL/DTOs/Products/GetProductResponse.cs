using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.DTOs.Products
{
    public class GetProductResponse
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal SellingPrice { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
    }
}
