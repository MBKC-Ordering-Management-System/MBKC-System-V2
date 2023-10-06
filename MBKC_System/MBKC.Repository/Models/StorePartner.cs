﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Models
{
    public class StorePartner
    {
        public int StoreId { get; set; }
        public int PartnerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        [ForeignKey("PartnerId")]
        public virtual Partner Partner { get; set; }
        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }
        public virtual IEnumerable<MappingProduct> MappingProducts { get; set; }
    }
}
