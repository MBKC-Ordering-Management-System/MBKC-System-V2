using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.GrabFood.Models
{
    public class GrabFoodModifierGroup
    {
        public int AvailableStatus { get; set; }
        public string ModifierGroupID { get; set; }
        public string ModifierGroupName { get; set; }
        public List<GrabFoodModifier> Modifiers { get; set; }
        public List<string> RelatedItemIDs { get; set; }
    }
}
