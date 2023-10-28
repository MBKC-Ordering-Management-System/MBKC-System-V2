using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class KitchenCenterMoneyExchange
    {
        public int ExchangeId { get; set; }
        public int KitchenCenterId { get; set; }

        [ForeignKey("KitchenCenterId")]
        public KitchenCenter KitchenCenter { get; set; }

        [ForeignKey("ExchangeId")]
        public virtual MoneyExchange MoneyExchange { get; set; }
    }
}
