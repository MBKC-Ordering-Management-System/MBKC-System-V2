using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.GrabFood.Models
{
    public class GrabFoodModifier
    {
        public int AvailableStatus { get; set; }
        public bool IsNeedExtraCost { get; set; }
        public string ModifierID { get; set; }
        public string ModifierName { get; set; }
        public decimal PriceInMin { get; set; }
        public int Quantity { get; set; }
        public int SortOrder { get; set; }
    }
}
