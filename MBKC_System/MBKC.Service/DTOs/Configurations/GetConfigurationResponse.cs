using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Configurations
{
    public class GetConfigurationResponse
    {
        public string Id { get; set; }
        public TimeSpan ScrawlingOrderStartTime { get; set; }
        public TimeSpan ScrawlingOrderEndTime { get; set; }
        public TimeSpan ScrawlingMoneyExchangeToKitchenCenter { get; set; }
        public TimeSpan ScrawlingMoneyExchangeToStore { get; set; }
    }
}
