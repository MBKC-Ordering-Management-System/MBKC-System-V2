using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class MoneyExchangeRepository
    {
        private MBKCDbContext _dbContext;
        public MoneyExchangeRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region Get
        //public async Task<List<MoneyExchange>> GetMoneyExchangesAsync()
        //{

        //}

        //public async Task<int> GetNumberOrdersAsync()
        //{

        //}

        #endregion

        #region Insert
        public async Task CreateMoneyExchangeAsync(MoneyExchange moneyExchange)
        {
            try
            {
                await this._dbContext.AddAsync(moneyExchange);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateRangeMoneyExchangeAsync(IEnumerable<MoneyExchange> moneyExchanges)
        {
            try
            {
                await this._dbContext.MoneyExchanges.AddRangeAsync(moneyExchanges);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
