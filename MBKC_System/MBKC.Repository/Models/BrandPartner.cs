using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class BrandPartner
    {
        public int BrandId { get; set; }
        public int PartnerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        [ForeignKey("PartnerId")]
        public virtual Partner Partner { get; set; }
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
    }
}
