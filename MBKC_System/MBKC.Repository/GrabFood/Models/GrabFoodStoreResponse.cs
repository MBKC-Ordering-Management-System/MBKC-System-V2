using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.GrabFood.Models
{
    public class GrabFoodStoreResponse
    {
        public string MerchantGroupID { get; set; }
        public List<GrabFoodStore> Merchants { get; set; }
    }
}
