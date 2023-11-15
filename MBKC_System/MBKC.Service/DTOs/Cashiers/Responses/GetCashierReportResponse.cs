using MBKC.Service.DTOs.KitchenCenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Cashiers.Responses
{
    public class GetCashierReportResponse
    {
        public GetCashierResponse Cashier { get; set; }
        public int? TotalOrderToday { get; set; }
        public decimal? TotalMoneyToday { get; set; }
        public decimal Balance { get; set; }
        public bool IsShiftEnded { get; set; }
    }
}
