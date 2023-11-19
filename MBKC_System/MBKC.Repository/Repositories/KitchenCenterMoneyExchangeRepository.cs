using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class KitchenCenterMoneyExchangeRepository
    {
        private MBKCDbContext _dbContext;
        public KitchenCenterMoneyExchangeRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<KitchenCenterMoneyExchange?> GetKitchenCenterMoneyExchangeAsync(int exchangeId)
        {
            return await this._dbContext.KitchenCenterMoneyExchanges.FirstOrDefaultAsync(kcme => kcme.ExchangeId == exchangeId);
        }

        public async Task CreateKitchenCenterMoneyExchangeAsync(KitchenCenterMoneyExchange kitchenCenterMoneyExchange)
        {
            try
            {
                await this._dbContext.AddAsync(kitchenCenterMoneyExchange);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateRangeKitchenCenterMoneyExchangeAsync(IEnumerable<KitchenCenterMoneyExchange> kitchenCenterMoneyExchanges)
        {
            try
            {
                await this._dbContext.KitchenCenterMoneyExchanges.AddRangeAsync(kitchenCenterMoneyExchanges);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
