using MBKC.DAL.DBContext;
using MBKC.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.DAOs
{
    public class RoleDAO
    {
        private MBKCDbContext _dbContext;
        public RoleDAO(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Role> GetRoleAsync(int roleId)
        {
            try
            {
                return await this._dbContext.Roles.SingleOrDefaultAsync(x => x.RoleId == roleId);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
