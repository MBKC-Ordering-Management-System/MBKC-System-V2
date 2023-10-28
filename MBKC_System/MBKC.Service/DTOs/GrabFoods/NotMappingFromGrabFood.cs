using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.GrabFoods
{
    public class NotMappingFromGrabFood
    {
        public List<NotMappingGrabFoodItem> NotMappingGrabFoodItems { get; set; }
        public List<NotMappingGrabFoodModifierGroup> NotMappingGrabFoodModifierGroups { get; set; }
    }
}
