﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.Cashiers.Requests
{
    public class GetCashierRequest
    {
        [FromRoute(Name = "id")]
        public int Id { get; set; }
    }
}
