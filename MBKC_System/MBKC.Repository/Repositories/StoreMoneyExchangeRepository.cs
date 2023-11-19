using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class StoreMoneyExchangeRepository
    {
        private MBKCDbContext _dbContext;
        public StoreMoneyExchangeRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateStoreMoneyExchangeAsync(StoreMoneyExchange storeMoneyExchange)
        {
            try
            {
                await this._dbContext.AddAsync(storeMoneyExchange);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<StoreMoneyExchange> GetStoreMoneyExchangeByStoreIdAsync(int storeId)
        {
            try
            {
                return await this._dbContext.StoreMoneyExchanges
                                                               .Include(x => x.Store)
                                                               .FirstOrDefaultAsync(x => x.StoreId == storeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateRangeStoreMoneyExchangeAsync(IEnumerable<StoreMoneyExchange> storeMoneyExchanges)
        {
            try
            {
                await this._dbContext.StoreMoneyExchanges.AddRangeAsync(storeMoneyExchanges);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
