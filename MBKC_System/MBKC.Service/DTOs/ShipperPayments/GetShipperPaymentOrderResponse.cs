using MBKC.Service.DTOs.BankingAccounts;
using MBKC.Service.DTOs.Transactions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.ShipperPayments
{
    public class GetShipperPaymentOrderResponse
    {
        public int PaymentId { get; set; }
        public int Status { get; set; }
        public string Content { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        public string CashierCreated { get; set; }
        public GetBankingAccountResponse BankingAccount { get; set; }
        public List<GetTransactionResponse> Transactions { get; set; }
    }
}
