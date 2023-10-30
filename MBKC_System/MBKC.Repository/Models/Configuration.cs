using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class Configuration
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TimeSpan ScrawlingOrderStartTime { get; set; }
        public TimeSpan ScrawlingOrderEndTime { get; set; }
        public TimeSpan ScrawlingMoneyExchangeToKitchenCenter { get; set; }
        public TimeSpan ScrawlingMoneyExchangeToStore { get; set; }
    }
}
