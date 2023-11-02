using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.OrdersHistories
{
    public class OrderHistoryResponse
    {
        public int OrderHistoryId { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SystemStatus { get; set; }
        public string PartnerOrderStatus { get; set; }
    }
}
