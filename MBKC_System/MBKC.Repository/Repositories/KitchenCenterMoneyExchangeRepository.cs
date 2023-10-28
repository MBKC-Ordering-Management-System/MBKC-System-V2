using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
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
