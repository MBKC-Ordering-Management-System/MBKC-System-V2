using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Orders
{
    public class GetOrdersResponse
    {
        public int NumberItems { get; set; }
        public int TotalPages { get; set; }
        public List<GetOrderResponse> Orders { get; set; }
    }
}