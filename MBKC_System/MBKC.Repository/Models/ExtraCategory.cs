using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class ExtraCategory
    { 
        [Key]
        public int CategoryId { get; set; }
        public int Status { get; set; }
        [ForeignKey("ExtraCategoryId")]
        public int ExtraCategoryId { get; set; }
        [ForeignKey("ProductCategoryId")]
        public int ProductCategoryId { get; set; }

        public virtual Category ExtraCategoryNavigation { get; set; }
        public virtual Category ProductCategory { get; set; }
    }
}
