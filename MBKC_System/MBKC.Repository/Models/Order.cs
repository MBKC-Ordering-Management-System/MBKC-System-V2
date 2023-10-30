using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string OrderPartnerId { get; set; }
        public string ShipperName { get; set; }
        public string ShipperPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Note { get; set; }
        public string PaymentMethod { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal SubTotalPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal FinalTotalPrice { get; set; }
        public float Commission { get; set; }
        public decimal Tax { get; set; }
        public string Status { get; set; }
        public int PartnerId { get; set; }
        public int StoreId { get; set; }
        public string DisplayId { get; set; }
        public string Address { get; set; }
        public int? Cutlery { get; set; }

        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }
        [ForeignKey("PartnerId")]
        public virtual Partner Partner { get; set; }
        public virtual IEnumerable<ShipperPayment> ShipperPayments { get; set; }
        public virtual IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
