using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<OrderRepository> haha = new Logger<OrderRepository>(new LoggerFactory());
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
                                                   .Include(o => o.OrderHistories)
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
                                                   .Include(o => o.OrderHistories)
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
            }
            catch (Exception ex)
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

        public async Task<int> GetNumberOrdersAsync(string? searchName, string? searchValueWithoutUnicode,
            int? storeId, int? kitchenCenterId, string? systemStatus, string? partnerOrderStatus, string? searchDateFrom, string? searchDateTo)
        {
            try
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                if (searchDateFrom != null && searchDateTo != null)
                {
                    startDate = DateTime.ParseExact(searchDateFrom, "dd/MM/yyyy", null);
                    endDate = DateTime.ParseExact(searchDateTo, "dd/MM/yyyy", null);
                }
                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.Orders.Include(x => x.Store)
                                                 .ThenInclude(x => x.KitchenCenter)
                                                         .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                     : true) && (systemStatus != null
                                                                     ? x.SystemStatus.Equals(systemStatus)
                                                                     : true) && (partnerOrderStatus != null
                                                                     ? x.PartnerOrderStatus.Equals(partnerOrderStatus)
                                                                     : true) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true))
                                                         .Where(delegate (Order order)
                                                         {
                                                             if (StringUtil.RemoveSign4VietnameseString(order.CustomerName).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).AsQueryable().Count();
                }
                else if (searchName != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.Orders.Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                                                       .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                     : true)
                                                                     && (systemStatus != null
                                                                     ? x.SystemStatus.Equals(systemStatus)
                                                                     : true) && (partnerOrderStatus != null
                                                                     ? x.PartnerOrderStatus.Equals(partnerOrderStatus)
                                                                     : true) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true) &&
                                                                     x.CustomerName.ToLower().Contains(searchName.ToLower())).CountAsync();
                }
                return await this._dbContext.Orders.Include(x => x.Store)
                                                   .ThenInclude(x => x.KitchenCenter)
                                                   .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                     : true) && (systemStatus != null
                                                                     ? x.SystemStatus.Equals(systemStatus)
                                                                     : true) && (partnerOrderStatus != null
                                                                     ? x.PartnerOrderStatus.Equals(partnerOrderStatus)
                                                                     : true) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true)).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Order>> GetOrdersAsync(string? searchValue, string? searchValueWithoutUnicode,
                                                      int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int? storeId,
                                                      int? kitchenCenterId, string? systemStatus, string? partnerOrderStatus, string? searchDateFrom, string? searchDateTo)
        {
            try
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                if (searchDateFrom != null && searchDateTo != null)
                {
                    startDate = DateTime.ParseExact(searchDateFrom, "dd/MM/yyyy", null);
                    endDate = DateTime.ParseExact(searchDateTo, "dd/MM/yyyy", null);
                }
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Orders.OrderByDescending(x => x.Id).Include(x => x.Store)
                                                     .Include(x => x.Partner)
                                                     .Include(x => x.ShipperPayments).ThenInclude(x => x.BankingAccount)
                                                                                     .ThenInclude(x => x.KitchenCenter).ThenInclude(x => x.Cashiers)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                     .Include(o => o.OrderHistories)
                                                     .Include(x => x.Store)
                                                     .ThenInclude(x => x.KitchenCenter)
                                                     .Where(x => (storeId != null
                                                                    ? x.StoreId == storeId
                                                                    : true) &&
                                                                    (kitchenCenterId != null
                                                                    ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                    : true) &&
                                                                    (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true))
                                                        .Where(delegate (Order order)
                                                        {
                                                            if (StringUtil.RemoveSign4VietnameseString(order.Store.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                            {
                                                                return true;
                                                            }
                                                            return false;
                                                        })
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("shippername"),
                                                                  then => then.OrderBy(x => x.ShipperName))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("customername"),
                                                                 then => then.OrderBy(x => x.CustomerName))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("totaldiscount"),
                                                                         then => then.OrderBy(x => x.TotalDiscount))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("finaltotalprice"),
                                                                         then => then.OrderBy(x => x.FinalTotalPrice))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("commission"),
                                                                         then => then.OrderBy(x => x.Commission))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("tax"),
                                                                         then => then.OrderBy(x => x.Tax))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                                                         then => then.OrderBy(x => x.Address))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                         then => then.OrderBy(x => x.Store.Name))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Orders.OrderByDescending(x => x.Id).Include(x => x.Store)
                                                     .Include(x => x.Partner)
                                                     .Include(x => x.ShipperPayments)
                                                     .Include(o => o.OrderHistories)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                     .Include(x => x.Store)
                                                     .ThenInclude(x => x.KitchenCenter)
                                                     .Where(x => (storeId != null
                                                                    ? x.StoreId == storeId
                                                                    : true) &&
                                                                    (kitchenCenterId != null
                                                                    ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                    : true) &&
                                                                    (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true))
                                                        .Where(delegate (Order order)
                                                        {
                                                            if (StringUtil.RemoveSign4VietnameseString(order.Store.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                            {
                                                                return true;
                                                            }
                                                            return false;
                                                        })
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("shippername"),
                                                                  then => then.OrderByDescending(x => x.ShipperName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("customername"),
                                                                 then => then.OrderByDescending(x => x.CustomerName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("totaldiscount"),
                                                                         then => then.OrderByDescending(x => x.TotalDiscount))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("finaltotalprice"),
                                                                         then => then.OrderByDescending(x => x.FinalTotalPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("commission"),
                                                                         then => then.OrderByDescending(x => x.Commission))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("tax"),
                                                                         then => then.OrderByDescending(x => x.Tax))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                                                         then => then.OrderByDescending(x => x.Address))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Orders.OrderByDescending(x => x.Id).Include(x => x.Store)
                                                 .Include(x => x.Partner)
                                                 .Include(o => o.OrderHistories)
                                                 .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                 .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                 .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                 .Include(x => x.OrderDetails)
                                                 .Include(x => x.Store)
                                                 .ThenInclude(x => x.KitchenCenter)
                                                    .Where(delegate (Order order)
                                                    {
                                                        if (StringUtil.RemoveSign4VietnameseString(order.Store.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                        {
                                                            return true;
                                                        }
                                                        return false;
                                                    })
                                                 .Where(x => (storeId != null
                                                                   ? x.StoreId == storeId
                                                                   : true) &&
                                                                   (kitchenCenterId != null
                                                                   ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                   : true) &&
                                                                    (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true))
                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Orders.OrderByDescending(x => x.Id).Include(x => x.Store)
                                                     .Include(x => x.Partner)
                                                     .Include(o => o.OrderHistories)
                                                     .Include(x => x.ShipperPayments)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                     .Include(x => x.Store)
                                                     .ThenInclude(x => x.KitchenCenter)
                                                           .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                     : true) &&
                                                                    (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     x.Store.Name.ToLower().Contains(searchValue.ToLower()) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("shippername"),
                                                                      then => then.OrderBy(x => x.ShipperName))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("customername"),
                                                              then => then.OrderBy(x => x.CustomerName))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("totaldiscount"),
                                                                      then => then.OrderBy(x => x.TotalDiscount))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("finaltotalprice"),
                                                                      then => then.OrderBy(x => x.FinalTotalPrice))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("commission"),
                                                                      then => then.OrderBy(x => x.Commission))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("tax"),
                                                                      then => then.OrderBy(x => x.Tax))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                                                         then => then.OrderBy(x => x.Address))
                                                           .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Orders.OrderByDescending(x => x.Id).Include(x => x.Store)
                                                     .Include(x => x.Partner)
                                                     .Include(o => o.OrderHistories)
                                                     .Include(x => x.ShipperPayments)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                     .Include(x => x.Store)
                                                     .ThenInclude(x => x.KitchenCenter)
                                                           .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                     : true) &&
                                                                      (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     x.Store.Name.ToLower().Contains(searchValue.ToLower()) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("shippername"),
                                                                  then => then.OrderByDescending(x => x.ShipperName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("customername"),
                                                                 then => then.OrderByDescending(x => x.CustomerName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("totaldiscount"),
                                                                         then => then.OrderByDescending(x => x.TotalDiscount))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("finaltotalprice"),
                                                                         then => then.OrderByDescending(x => x.FinalTotalPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("commission"),
                                                                         then => then.OrderByDescending(x => x.Commission))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("tax"),
                                                                         then => then.OrderByDescending(x => x.Tax))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                                                         then => then.OrderByDescending(x => x.Address))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Orders.OrderByDescending(x => x.Id).Include(x => x.Store)
                                                     .Include(x => x.Partner)
                                                     .Include(x => x.ShipperPayments)
                                                     .Include(o => o.OrderHistories)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                     .Include(x => x.Store)
                                                     .ThenInclude(x => x.KitchenCenter)
                                                           .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                     : true) &&
                                                                       (systemStatus != null
                                                                     ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                     : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     x.Store.Name.ToLower().Contains(searchValue.ToLower()) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                if (sortByASC is not null)
                    return this._dbContext.Orders.OrderByDescending(x => x.Id).Include(x => x.Store)
                                                     .Include(x => x.Partner)
                                                     .Include(x => x.ShipperPayments)
                                                     .Include(o => o.OrderHistories)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                     .Include(x => x.Store)
                                                     .ThenInclude(x => x.KitchenCenter)
                                                           .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                     : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("shippername"),
                                                                      then => then.OrderBy(x => x.ShipperName))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("customername"),
                                                              then => then.OrderBy(x => x.CustomerName))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("totaldiscount"),
                                                                      then => then.OrderBy(x => x.TotalDiscount))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("finaltotalprice"),
                                                                      then => then.OrderBy(x => x.FinalTotalPrice))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("commission"),
                                                                      then => then.OrderBy(x => x.Commission))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("tax"),
                                                                      then => then.OrderBy(x => x.Tax))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                                                         then => then.OrderBy(x => x.Address))
                                                           .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                else if (sortByDESC is not null)
                    return this._dbContext.Orders.OrderByDescending(x => x.Id).Include(x => x.Store)
                                                     .Include(x => x.Partner)
                                                     .Include(x => x.ShipperPayments)
                                                     .Include(o => o.OrderHistories)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                     .Include(x => x.Store)
                                                     .ThenInclude(x => x.KitchenCenter)
                                                           .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                     : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("shippername"),
                                                                  then => then.OrderByDescending(x => x.ShipperName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("customername"),
                                                                 then => then.OrderByDescending(x => x.CustomerName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("totaldiscount"),
                                                                         then => then.OrderByDescending(x => x.TotalDiscount))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("finaltotalprice"),
                                                                         then => then.OrderByDescending(x => x.FinalTotalPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("commission"),
                                                                         then => then.OrderByDescending(x => x.Commission))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("tax"),
                                                                         then => then.OrderByDescending(x => x.Tax))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                                                         then => then.OrderByDescending(x => x.Address))
                                                           .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                return this._dbContext.Orders.OrderByDescending(x => x.Id).Include(x => x.Store)
                                                     .Include(x => x.Partner)
                                                     .Include(x => x.ShipperPayments)
                                                     .Include(o => o.OrderHistories)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                     .Include(x => x.Store)
                                                     .ThenInclude(x => x.KitchenCenter)
                                                           .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId
                                                                     : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() >= startDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Month).SingleOrDefault() <= endDate.Month &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Year).SingleOrDefault() <= endDate.Year : true))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<Order> GetOrderAsync(int id)
        {
            return await this._dbContext.Orders
                             .Include(x => x.Store)
                             .Include(x => x.Partner)
                             .Include(x => x.ShipperPayments).ThenInclude(x => x.BankingAccount)
                             .Include(x => x.OrderHistories)
                             .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                             .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                             .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                             .Include(x => x.Store).SingleOrDefaultAsync(x => x.Id == id);

        }
    }
}
