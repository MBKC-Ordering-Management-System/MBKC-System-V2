using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class CashierMoneyExchangeRepository
    {
        private MBKCDbContext _dbContext;
        public CashierMoneyExchangeRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateCashierMoneyExchangeAsync(CashierMoneyExchange cashierMoneyExchange)
        {
            try
            {
                await this._dbContext.AddAsync(cashierMoneyExchange);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
