using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Categories
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
