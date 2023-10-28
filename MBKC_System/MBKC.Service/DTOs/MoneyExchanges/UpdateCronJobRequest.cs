using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.MoneyExchanges
{
    public class UpdateCronJobRequest
    {
        public string JobId { get; set; }
        public int hour {  get; set; }
        public int minute { get; set; }
    }
}
