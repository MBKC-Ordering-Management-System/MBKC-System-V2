using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.GrabFood.Models
{
    public class GrabFoodCategory
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int AvailableStatus { get; set; }
        public int SortOrder { get; set; }
        public List<GrabFoodItem> Items { get; set; }
    }
}
