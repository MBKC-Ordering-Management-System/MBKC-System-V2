using MBKC.Repository.GrabFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.GrabFoods
{
    public class NotMappingGrabFoodItem
    {
        public GrabFoodItem GrabFoodItem { get; set; }
        public string Reason { get; set; }
    }
}
