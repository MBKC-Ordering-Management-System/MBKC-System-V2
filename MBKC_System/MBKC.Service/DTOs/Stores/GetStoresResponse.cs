using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Stores
{
    public class GetStoresResponse
    {
        public int NumberItems { get; set; }
        public int TotalPages { get; set; }
        public List<GetStoreResponse> Stores { get; set; }
    }
}
