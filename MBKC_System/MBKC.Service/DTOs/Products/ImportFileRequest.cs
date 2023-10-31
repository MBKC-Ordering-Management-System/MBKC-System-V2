using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Products
{
    public class ImportFileRequest
    {
        public IFormFile file { get; set; }
    }
}
