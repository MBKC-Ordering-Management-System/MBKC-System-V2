using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.PartnerProducts
{
    public class PartnerProductRequest
    {
        [FromRoute(Name = "productId")]
        public int ProductId { get; set; }
        [FromRoute(Name = "partnerId")]
        public int PartnertId { get; set; }
        [FromRoute(Name = "storeId")]
        public int StoreId { get; set; }
    }
}
