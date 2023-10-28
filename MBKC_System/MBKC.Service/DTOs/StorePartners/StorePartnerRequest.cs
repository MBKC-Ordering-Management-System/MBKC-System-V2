using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.StorePartners
{
    public class StorePartnerRequest
    {
        [FromRoute(Name = "storeId")]
        public int StoreId { get; set; }
        [FromRoute(Name = "partnerId")]
        public int PartnerId { get; set; }
    }
}
