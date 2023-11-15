using MBKC.Service.DTOs.Cashiers.Responses;
using MBKC.Service.DTOs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards
{
    public class GetKitchenCenterDashBoardResponse
    {
        public int TotalStore { get; set; }
        public int TotalCashier { get; set; }
        public decimal TotalBalanceDaily { get; set; }
        public List<GetColumnChartResponse>? ColumnChartMoneyExchanges { get; set; }
        public List<GetStoreResponse>? Stores { get; set; }
        public List<GetCashierResponse>? Cashiers { get; set; }
    }
}
