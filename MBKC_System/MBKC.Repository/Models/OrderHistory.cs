using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class OrderHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderHistoryId { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SystemStatus { get; set; }
        public string PartnerOrderStatus { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}
