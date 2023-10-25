using MBKC.Repository.GrabFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.GrabFoods
{
    public class NotMappingGrabFoodModifierGroup
    {
        public GrabFoodModifierGroup GrabFoodModifierGroup { get; set; }
        public string Reason { get; set; }
    }
}
