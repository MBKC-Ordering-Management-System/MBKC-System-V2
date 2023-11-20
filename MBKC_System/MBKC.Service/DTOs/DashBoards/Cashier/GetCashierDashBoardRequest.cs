using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards.Cashier
{
    public class GetCashierDashBoardRequest
    {
        public string? OrderSearchDateFrom { get; set; }
        public string? OrderSearchDateTo { get; set; }
    }
}
