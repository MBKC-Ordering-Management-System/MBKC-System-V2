using MBKC.DAL.DBContext;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.DAOs
{
    public class BrandDAO
    {
        private MBKCDbContext _dbContext;
        public BrandDAO(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateBrand(Brand brand)
        {
            try
            {
                await this._dbContext.Brands.AddAsync(brand);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
