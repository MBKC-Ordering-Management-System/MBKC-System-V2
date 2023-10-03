using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Categories
{
    public class PostCategoryRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int DisplayOrder { get; set; }
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
