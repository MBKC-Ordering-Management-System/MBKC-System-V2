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
                    ScrawlingOrderStartTime = new TimeSpan(0, 0, 0),
                    ScrawlingOrderEndTime = new TimeSpan(21, 50, 0),
                    ScrawlingMoneyExchangeToKitchenCenter = new TimeSpan(22, 0, 0),
                    ScrawlingMoneyExchangeToStore = new TimeSpan(23, 0, 0),
                }
             );
        }
    }
}
