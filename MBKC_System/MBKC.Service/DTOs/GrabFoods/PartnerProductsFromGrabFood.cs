using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.GrabFoods
{
    public class PartnerProductsFromGrabFood
    {
        public List<PartnerProduct> NewPartnerProducts { get; set; }
        public List<PartnerProduct> OldPartnerProducts { get; set; }
        public NotMappingFromGrabFood NotMappingFromGrabFood { get; set; }
    }
}
