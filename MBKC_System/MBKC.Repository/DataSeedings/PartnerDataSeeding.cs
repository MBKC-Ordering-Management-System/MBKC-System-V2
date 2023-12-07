using MBKC.Repository.Constants;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.DataSeedings
{
    public static class PartnerDataSeeding
    {
        public static void PartnerData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Partner>().HasData(
                new Partner()
                {
                    PartnerId = 1,
                    Name = PartnerConstant.GrabFood,
                    Status = (int)PartnerEnum.Status.ACTIVE,
                    TaxCommission = 10
                }
            );
        }
    }
}
