using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards
{
    public class GetStoreDashBoardResponse
    {
        public int TotalUpcomingOrders { get; set; }
        public int TotalPreparingOrders { get; set; }
        public int TotalReadyOrders { get; set; }
        public int TotalCompletedOrders { get; set; }
        public decimal TotalRevenuesDaily { get; set; }
    }
}
