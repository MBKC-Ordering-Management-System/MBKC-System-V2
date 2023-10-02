using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class BrandAccountRepository
    {
        private MBKCDbContext _dbContext;
        public BrandAccountRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateBrandAccount(BrandAccount brandAccount)
        {
            try
            {
                await this._dbContext.BrandAccounts.AddAsync(brandAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
    }

    }
}
