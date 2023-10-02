using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class Cashier
    {
        [Key]
        public int AccountId { get; set; }
        public string FullName { get; set; }
        public bool Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Avatar { get; set; }
        public string CitizenNumber { get; set; }
        public int WalletId { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        [ForeignKey("KitchenCenterId")]
        public virtual KitchenCenter KitchenCenter { get; set; }
        [ForeignKey("WalletId")]
        public virtual Wallet Wallet { get; set; }
        public virtual IEnumerable<CashierMoneyExchange> CashierMoneyExchanges { get; set; }    
    }
}
