using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<StoreAccount> GetStoreAccountAsync(int accountId)
        {
            try
            {
                return await this._dbContext.StoreAccounts.SingleOrDefaultAsync(x => x.AccountId == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
