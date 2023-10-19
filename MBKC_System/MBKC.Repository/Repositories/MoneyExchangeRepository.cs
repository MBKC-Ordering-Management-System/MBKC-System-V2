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
    }
}
