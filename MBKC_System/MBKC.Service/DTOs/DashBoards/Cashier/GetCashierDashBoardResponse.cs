using MBKC.Service.DTOs.MoneyExchanges;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.DTOs.ShipperPayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.DashBoards.Cashier
{
    public class GetCashierDashBoardResponse
    {
        public decimal TotalRevenuesDaily { get; set; }
        public int TotalOrdersDaily { get; set; }
        public List<GetOrderResponse>? Orders { get; set; }
        public List<GetMoneyExchangeResponse>? MoneyExchanges { get; set; }
        public List<GetShipperPaymentResponse>? ShipperPayments { get; set; }
    }
}
