using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.StorePartners
{
    public class StoreRequest
    {
        [FromRoute(Name = "storeId")]
        public int StoreId { get; set; }
    }
}
