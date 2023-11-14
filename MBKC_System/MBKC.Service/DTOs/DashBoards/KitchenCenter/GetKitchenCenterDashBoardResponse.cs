using MBKC.Service.DTOs.Cashiers.Responses;
using MBKC.Service.DTOs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards.KitchenCenter
{
    public class GetKitchenCenterDashBoardResponse
    {
        public int TotalStore { get; set; }
        public int TotalCashier { get; set; }
        public decimal TotalBalance { get; set; }
        public List<decimal>? Benefits { get; set; }
        public List<GetStoreResponse>? Stores { get; set; }
        public List<GetCashierResponse>? Cashiers { get; set; }
    }
}
