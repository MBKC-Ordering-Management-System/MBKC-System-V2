using MBKC.Repository.Models;
using MBKC.Service.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.OrderDetails
{
    public class GetOrderDetailResponse
    {
        public int OrderDetailId { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public int OrderId { get; set; }
        public GetOrderDetailResponse? MasterOrderDetail { get; set; }
        public GetProductResponse Product { get; set; }
        public List<GetOrderDetailResponse>? ExtraOrderDetails { get; set; }
    }
}
