using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class StoreAccountRepository
    {
        private MBKCDbContext _dbContext;
        public StoreAccountRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task AddStoreAccountAsync(StoreAccount storeAccount)
        {
            try
            {
                await this._dbContext.StoreAccounts.AddAsync(storeAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
