using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class StoreAccount
    {
        public int StoreId { get; set; }
        public int AccountId { get; set; }
        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
    }
}
