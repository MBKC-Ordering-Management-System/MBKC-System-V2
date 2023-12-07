using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class OrderDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailId { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public decimal DiscountPrice { get; set; }
        public int? MasterOrderDetailId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("MasterOrderDetailId")]
        public virtual OrderDetail? MasterOrderDetail { get; set; }
        public virtual IEnumerable<OrderDetail>? ExtraOrderDetails { get; set; }
    }
}
