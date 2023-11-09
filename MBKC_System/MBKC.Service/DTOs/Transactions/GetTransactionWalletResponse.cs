using MBKC.Service.DTOs.MoneyExchanges;
using MBKC.Service.DTOs.ShipperPayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Transactions
{
    public class GetTransactionWalletResponse
    {
        public int TracsactionId { get; set; }
        public DateTime TransactionTime { get; set; }
        public string Status { get; set; }
        public GetMoneyExchangeResponse? MoneyExchange { get; set; }
        public GetShipperPaymentResponse? ShipperPayment { get; set; }
    }
}
