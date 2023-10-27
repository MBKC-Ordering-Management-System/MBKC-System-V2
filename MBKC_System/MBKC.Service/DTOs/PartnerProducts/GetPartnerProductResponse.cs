using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.PartnerProducts
{
    public class GetPartnerProductResponse
    {
        public int ProductId { get; set; }
        public int PartnerId { get; set; }
        public int StoreId { get; set; }
        public string ProductName { get; set; }
        public string PartnerName { get; set; }
        public string StoreName { get; set; }
        public string ProductCode { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public DateTime MappedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
