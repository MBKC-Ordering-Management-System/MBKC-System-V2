using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.GrabFood.Models
{
    public class GrabFoodProduct
    {
        public string? ItemCode { get; set; }
        public int AvailableStatus { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal PriceInMin { get; set; }
        public int SortOrder { get; set; }
        public string ImageURL { get; set; }
        public bool SoldByWeight { get; set; }
        public double? Weight { get; set; }
        public List<string> LinkedModifierGroupIDs { get; set; }
    }
}
