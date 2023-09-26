using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.DTOs.Stores
{
    public class GetStoresResponse
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<GetStoreResponse> Stores { get; set; }
    }
}
