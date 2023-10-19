using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.GrabFood.Models
{
    public class GrabFoodMenu
    {
        public List<GrabFoodCategory> Categories { get; set; }
        public bool IsMenuEditorEnabled { get; set; }
        public List<GrabFoodModifierGroup> ModifierGroups { get; set; }
    }
}
