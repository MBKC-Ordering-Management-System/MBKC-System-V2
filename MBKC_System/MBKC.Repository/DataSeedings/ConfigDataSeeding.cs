using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.DataSeedings
{
    public static class ConfigDataSeeding
    {
        public static void ConfigData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Configuration>().HasData(
                new Configuration()
                {
                    Id = 1,
                    ScrawlingOrderStartTime = TimeSpan.FromHours(18),
                    ScrawlingOrderEndTime = TimeSpan.FromHours(18),
                    TimeMoneyExchangeToKitcenCenter = "* 22 * * *",
                    TimeMoneyExchangeToStore = "* 23 * * *"
                }
             );
        }
    }
}
