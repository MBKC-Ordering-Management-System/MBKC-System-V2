using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.OrderDetails
{
    public class PostOrderDetailRequest
    {
        public int ProductId { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public List<PostOrderDetailRequest>? ExtraOrderDetails { get; set; }
    }
}
