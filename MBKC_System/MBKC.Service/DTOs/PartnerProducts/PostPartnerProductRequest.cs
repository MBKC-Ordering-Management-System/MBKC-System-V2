using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.PartnerProducts
{
    public class PostPartnerProductRequest
    {
        public int ProductId { get; set; }
        public int PartnerId { get; set; }
        public int StoreId { get; set; }
        public string ProductCode { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
    }
}
