using MBKC.Service.DTOs.KitchenCenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Cashiers.Responses
{
    public class GetCashierResponse
    {
        public int AccountId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Avatar { get; set; }
        public string CitizenNumber { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public GetKitchenCenterResponse KitchenCenter { get; set; }
    }
}
