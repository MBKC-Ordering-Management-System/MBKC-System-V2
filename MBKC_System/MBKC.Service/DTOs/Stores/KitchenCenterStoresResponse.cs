using MBKC.Service.DTOs.Brands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Stores
{
    public class KitchenCenterStoresResponse
    {
        public int StoreId { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string Logo { get; set; }
        public string StoreManagerEmail { get; set; }
        public StoreBrandResponse Brand { get; set; }
    }
}
