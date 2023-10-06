using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.MappingProducts
{
    public class PostMappingProductRequest
    {
        public int ProductId { get; set; }
        public int PartnerId { get; set; }
        public int StoreId { get; set; }
        public string ProductCode { get; set; }
    }
}
