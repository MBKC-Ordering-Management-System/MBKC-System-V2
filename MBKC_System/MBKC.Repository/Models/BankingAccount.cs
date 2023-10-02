using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class BankingAccount
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BankingAccountId { get; set; }
        public string NumberAccount { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        [ForeignKey("KitchenCenterId")]
        public int KitchenCenterId { get; set; }
        public virtual KitchenCenter KitchenCenter { get; set; }
        public virtual IEnumerable<ShipperPayment> ShipperPayments { get; set; }    
    }
}
