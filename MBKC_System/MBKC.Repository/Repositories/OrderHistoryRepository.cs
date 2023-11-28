using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class OrderHistoryRepository
    {
        private MBKCDbContext _dbContext;
        public OrderHistoryRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task InsertOrderHistoryAsync(OrderHistory orderHistory)
        {
            try
            {
                await this._dbContext.OrderHistories.AddAsync(orderHistory);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task InsertRangeOrderHistoryAsync(IEnumerable<OrderHistory> orderHistories)
        {
            try
            {
                await this._dbContext.OrderHistories.AddRangeAsync(orderHistories);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
