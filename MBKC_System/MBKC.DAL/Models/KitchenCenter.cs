using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Models
{
    public class KitchenCenter
    {
        [Key]
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Status { get; set; }
        public string Logo { get; set; }
        public int ManagerAccount { get; set; }
        public int WalletId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        [ForeignKey("WalletId")]
        public virtual Wallet Wallet { get; set; }  
        public virtual IEnumerable<Cashier> Cashiers { get; set; }
        public virtual IEnumerable<KitchenCenterMoneyExchange> KitchenCenterMoneyExchanges { get; set; }
        public virtual IEnumerable<BankingAccount> BankingAccounts { get; set; }
        public virtual IEnumerable<Store> Stores { get; set; }

    }
}
