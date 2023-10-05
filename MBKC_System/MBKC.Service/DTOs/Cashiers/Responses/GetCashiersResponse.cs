using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Cashiers.Responses
{
    public class GetCashiersResponse
    {
        public int TotalPages { get; set; }
        public int NumberItems { get; set; }
        public List<GetCashierResponse> Cashiers { get; set; }
    }
}
