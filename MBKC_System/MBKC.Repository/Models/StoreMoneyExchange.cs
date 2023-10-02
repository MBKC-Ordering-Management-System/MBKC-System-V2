using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class StoreMoneyExchange
    {
        public int ExchangeId { get; set; }
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store Store { get; set; }
        [ForeignKey("ExchangeId")]
        public virtual MoneyExchange MoneyExchange { get; set; }
    }
}
