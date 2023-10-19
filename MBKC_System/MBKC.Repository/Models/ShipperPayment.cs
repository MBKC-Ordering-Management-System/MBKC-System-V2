using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class ShipperPayment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }
        public int Status { get; set; }
        public string Content { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        public string PaymentMethod { get; set; }
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public int? KCBankingAccountId { get; set; }

        [ForeignKey("KCBankingAccountId")]
        public BankingAccount? BankingAccount { get; set; }
        public Order Order { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
