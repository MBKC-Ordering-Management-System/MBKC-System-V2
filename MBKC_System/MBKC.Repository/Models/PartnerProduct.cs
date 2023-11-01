using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class PartnerProduct
    {
        public int ProductId { get; set; }
        public int PartnerId { get; set; }
        public int StoreId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime MappedDate { get; set; }
        public string ProductCode { get; set; }
        public int Status { get; set; }
        public decimal Price { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("StoreId,PartnerId,CreatedDate")]
        public virtual StorePartner StorePartner { get; set; }
    }
}
