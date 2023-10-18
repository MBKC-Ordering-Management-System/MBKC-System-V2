using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public string? Size { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public string Image { get; set; }
        public int DisplayOrder { get; set; }
        public int? ParentProductId { get; set; }
        public decimal HistoricalPrice { get; set; }

        
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
        public virtual IEnumerable<OrderDetail> OrderDetails { get; set; }
        public virtual IEnumerable<PartnerProduct> PartnerProducts { get; set; }
        [ForeignKey("ParentProductId")]
        public virtual Product? ParentProduct { get; set; }
        public virtual IEnumerable<Product>? ChildrenProducts { get; set; }

    }
}
