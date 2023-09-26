using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.DTOs.Stores
{
    public class UpdateStoreRequest
    {
        public string Name { get; set; }
        public IFormFile? NewLogo { get; set; }
        public string? DeletedLogo { get; set; }
        public string StoreManagerEmail { get; set; }
        public string Status { get; set; }
    }
}
