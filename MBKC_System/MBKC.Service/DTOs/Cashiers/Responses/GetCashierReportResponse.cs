using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Cashiers.Responses
{
    public class GetCashierReportResponse
    {
        public string CashierName { get; set; }
        public string KitchenCenterName { get; set; }
        public int? TotalOrderToday { get; set; }
        public decimal? TotalMoneyToday { get; set; }
        public decimal Balance { get; set; }
    }
}
