using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public async Task<Order> GetOrderByOrderPartnerIdAsync(string orderPartnerId)
        {
            try
            {
                return await this._dbContext.Orders.Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                   .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                   .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                   .Include(o => o.Partner)
                                                   .Include(o => o.ShipperPayments).ThenInclude(o => o.BankingAccount)
                                                   .Include(o => o.ShipperPayments).ThenInclude(o => o.Transactions)
                                                   .Include(o => o.Store).ThenInclude(o => o.KitchenCenter)
                                                   .FirstOrDefaultAsync(o => o.OrderPartnerId.Equals(orderPartnerId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public async Task<Order> GetOrderByDisplayIdAsync(string displayId)
        {
            try
            {
                return await this._dbContext.Orders.Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                   .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                   .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                   .Include(o => o.Partner)
                                                   .Include(o => o.ShipperPayments).ThenInclude(o => o.BankingAccount)
                                                   .Include(o => o.ShipperPayments).ThenInclude(o => o.Transactions)
                                                   .Include(o => o.Store).ThenInclude(o => o.KitchenCenter)
                                                   .FirstOrDefaultAsync(o => o.DisplayId.Equals(displayId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task InsertOrderAsync(Order order)
        {
            try
            {
                await this._dbContext.Orders.AddAsync(order);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

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
