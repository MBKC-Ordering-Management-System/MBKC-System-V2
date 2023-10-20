using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class CashierMoneyExchange
    {
        public int ExchangeId;

        public int CashierId;

        [ForeignKey("CashierId")]
        public virtual Cashier Cashier { get; set; }

        [ForeignKey("ExchangeId")]
        public virtual MoneyExchange MoneyExchange { get; set; }
    }
}
