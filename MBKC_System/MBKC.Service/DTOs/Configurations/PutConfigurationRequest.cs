using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Configurations
{
    public class PutConfigurationRequest
    {
        public string ScrawlingOrderStartTime { get; set; }
        public string ScrawlingOrderEndTime { get; set; }
    }
}
