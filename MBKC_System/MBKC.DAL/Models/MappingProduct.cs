using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Models
{
    public class MappingProduct
    {
        public int ProductId { get; set; }
        [ForeignKey("PartnerId")]
        public int PartnerId { get; set; }
        [ForeignKey("StoreId")]
        public int StoreId { get; set; }
        public string ProductCode { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public virtual StorePartner StorePartner { get; set; }
    }
}
