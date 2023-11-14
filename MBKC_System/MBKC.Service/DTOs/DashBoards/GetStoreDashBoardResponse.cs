using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards
{
    public class GetStoreDashBoardResponse
    {
        public int TotalUpcomingOrder { get; set; }
        public int TotalPreparingOrder { get; set; }
        public int TotalReadyOrder { get; set; }
        public int TotalCompletedOrder { get; set; }
        public decimal TotalRevenueDaily { get; set; }
    }
}
