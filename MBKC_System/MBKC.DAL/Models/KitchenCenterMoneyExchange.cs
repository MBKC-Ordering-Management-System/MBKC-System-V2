using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Models
{
    public class KitchenCenterMoneyExchange
    {
        [Key]
        public int ExchangeId { get; set; }
        [ForeignKey("KitchenCenterId")]
        public int KitchenCenterId { get; set; }
        public KitchenCenter KitchenCenter { get; set; }
        [ForeignKey("ExchangeId")]
        public virtual MoneyExchange MoneyExchange { get; set; }
    }
}
