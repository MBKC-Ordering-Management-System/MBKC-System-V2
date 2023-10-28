using MBKC.Service.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.PartnerProducts
{
    public class GetPartnerProductForPrivateAPI
    {
        public int ProductId { get; set; }
        public int PartnerId { get; set; }
        public int StoreId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProductCode { get; set; }
        public decimal Price { get; set; }
        public GetProductResponse Product { get; set; }
    }
}
