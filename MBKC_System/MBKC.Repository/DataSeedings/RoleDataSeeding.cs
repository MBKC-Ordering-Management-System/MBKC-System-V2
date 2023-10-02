using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.DataSeedings
{
   public static class RoleDataSeeding
    {
        public static void RoleData (this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role() { RoleId = 1, RoleName = "MBKC Admin"},
                new Role() { RoleId = 2, RoleName = "Brand Manager"},
                new Role() { RoleId = 3, RoleName = "Kitchen Center Manager"},
                new Role() { RoleId = 4, RoleName = "Store Manager"},
                new Role() { RoleId = 5, RoleName = "Cashier"}
            );
        }
    }
}
