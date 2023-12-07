using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class Partner
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PartnerId { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string? WebUrl { get; set; }
        public int Status { get; set; }
        public float TaxCommission { get; set; }

        public virtual IEnumerable<Order> Orders { get; set; }
        public virtual IEnumerable<StorePartner> StorePartners { get; set; }
    }
}
