using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class MoneyExchange
    {
        [Key]
        public int ExchangeId { get; set; }
        public decimal Amount { get; set; }
        public string ExchangeType { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public int SenderId { get; set; }
        public int ReceiveId { get; set; }
        public string? ExchangeImage { get; set; }
        public virtual IEnumerable<Transaction> Transactions { get; set; }
    }
}
