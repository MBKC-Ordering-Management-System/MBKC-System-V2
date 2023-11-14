using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards.KitchenCenter
{
    public class GetColumnChartMoneyExchangesResponse
    {
        public DateTime Date {  get; set; }
        public decimal Amount { get; set; }
    }
}
