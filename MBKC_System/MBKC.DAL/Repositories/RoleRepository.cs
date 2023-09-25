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
    public class RoleRepository
    {
        private MBKCDbContext _dbContext;
        public RoleRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Role> GetRoleById(int id)
        {
            try
            {
                return await _dbContext.Roles.SingleOrDefaultAsync(r => r.RoleId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
