using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.MoneyExchanges
{
    public class GetMoneyExchangeResponse
    {
        public int ExchangeId { get; set; }
        public decimal Amount { get; set; }
        public string ExchangeType { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int ReceiveId { get; set; }
        public string ReceiveName { get; set; }
        public string? ExchangeImage { get; set; }
        public DateTime TransactionTime { get; set; }
    }
}
