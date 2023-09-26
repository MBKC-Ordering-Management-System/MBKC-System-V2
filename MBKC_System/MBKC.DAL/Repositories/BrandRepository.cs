using MBKC.DAL.DBContext;
using MBKC.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Repositories
{
    public class BrandRepository
    {
        private MBKCDbContext _dbContext;
        public BrandRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Brand> GetBrandAsync(int id)
        {
            try
            {
                return await this._dbContext.Brands.FirstOrDefaultAsync(x => x.BrandId == id);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
