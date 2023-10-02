using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class Category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int DisplayOrder { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Status { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
        public virtual IEnumerable<Product> Products { get; set; }
        public virtual IEnumerable<ExtraCategory> ExtraCategoryExtraCategoryNavigations { get; set; }
        public virtual IEnumerable<ExtraCategory> ExtraCategoryProductCategories { get; set; }

    }
}
