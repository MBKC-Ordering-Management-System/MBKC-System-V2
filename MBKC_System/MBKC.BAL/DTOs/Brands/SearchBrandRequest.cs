using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.DTOs.Brands
{
    public class SearchBrandRequest
    {
        public string? KeySearchName { get; set; }
        public string? KeyStatusFilter { get; set; }
    }
}
