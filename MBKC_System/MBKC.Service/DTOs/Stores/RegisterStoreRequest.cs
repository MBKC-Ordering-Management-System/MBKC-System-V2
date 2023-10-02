using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Stores
{
    public class RegisterStoreRequest
    {
        public string Name { get; set; }
        public IFormFile Logo { get; set; }
        public int KitchenCenterId { get; set; }
        public int BrandId { get; set; }
        public string StoreManagerEmail { get; set; }
    }
}
