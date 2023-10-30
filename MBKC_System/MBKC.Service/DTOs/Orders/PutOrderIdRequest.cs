using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Orders
{
    public class PutOrderIdRequest
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }
    }
}
