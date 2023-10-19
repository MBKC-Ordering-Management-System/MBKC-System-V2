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

    public class OrderRepository
    {
        private MBKCDbContext _dbContext;
        public OrderRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region get order by order partner id
        public async Task<Order> GetOrderByOrderPartnerIdAsync(String orderPartnerId)
        {
            try
            {
                return await this._dbContext.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderPartnerId == orderPartnerId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region update order
        public void UpdateOrder(Order order)
        {
            try
            {
                this._dbContext.Orders.Update(order);   
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

    }
}
