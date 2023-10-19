using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.GrabFood.Models
{
    public class GrabFoodAPI
    {
        public string AuthenticationURI { get; set; }
        public string StoresURI { get; set; }
        public string MenusURI { get; set; }
        public string RequestSource { get; set; }
    }
}
