using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class Store
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StoreId { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string Logo { get; set; }
        public string StoreManagerEmail { get; set; }
        public string? RejectedReason { get; set; }
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
        [ForeignKey("KitchenCenterId")]
        public virtual KitchenCenter KitchenCenter { get; set; }
        [ForeignKey("WalletId")]
        public virtual Wallet? Wallet { get; set; }
        public virtual IEnumerable<StoreAccount> StoreAccounts { get; set; }
        public virtual IEnumerable<StoreMoneyExchange> StoreMoneyExchanges { get; set; }
        public virtual IEnumerable<Order> Orders { get; set; }
        public virtual IEnumerable<StorePartner> StorePartners { get; set; }
    }
}
