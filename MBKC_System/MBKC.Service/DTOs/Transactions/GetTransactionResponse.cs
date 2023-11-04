using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Transactions
{
    public class GetTransactionResponse
    {
        public int TransactionId { get; set; }
        public DateTime TransactionTime { get; set; }
        public string Status { get; set; }
    }
}
