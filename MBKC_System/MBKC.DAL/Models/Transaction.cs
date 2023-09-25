using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Models
{
    public class Transaction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TracsactionId { get; set; }
        public DateTime TransactionTime { get; set; }
        public int Status { get; set; }
        [ForeignKey("ExchangeId")]
        public virtual MoneyExchange MoneyExchange { get; set; }
        [ForeignKey("WalletId")]
        public virtual Wallet Wallet { get; set; }
        [ForeignKey("PaymentId")]
        public virtual ShipperPayment ShipperPayment { get; set; }
    }
}
