﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.DTOs.StorePartners
{
    public class UpdateStorePartnerRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
    }
}