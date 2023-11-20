using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class UserDevice
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserDeviceId { get; set; }
        public string FCMToken { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
    }
}
