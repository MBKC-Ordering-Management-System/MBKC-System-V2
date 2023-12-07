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
                                                   .Include(o => o.Store).ThenInclude(o => o.StorePartners)
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

        #region Get number oders
        public async Task<int> GetNumberOrdersAsync(string? searchName, string? searchValueWithoutUnicode,
            int? storeId, int? kitchenCenterId, string? systemStatus, string? partnerOrderStatus, string? searchDateFrom, string? searchDateTo, int? cashierId, bool? confirmedBy)
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
                else if (searchDateFrom != null && searchDateTo == null)
                {
                    startDate = DateTime.ParseExact(searchDateFrom, "dd/MM/yyyy", null);
                }
                else if (searchDateFrom == null && searchDateTo != null)
                {
                    endDate = DateTime.ParseExact(searchDateTo, "dd/MM/yyyy", null);
                }
                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.Orders.Include(x => x.Store)
                                                 .ThenInclude(x => x.KitchenCenter)
                                                         .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                       (systemStatus != null
                                                                     ? x.SystemStatus.Equals(systemStatus)
                                                                     : true) && (partnerOrderStatus != null
                                                                     ? x.PartnerOrderStatus.Equals(partnerOrderStatus)
                                                                     : true) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                      && (searchDateFrom != null && searchDateTo == null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                      && (searchDateFrom == null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true))
                                                         .Where(delegate (Order order)
                                                         {
                                                             if (StringUtil.RemoveSign4VietnameseString(order.OrderPartnerId).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
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
                                                                     (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                     (systemStatus != null
                                                                     ? x.SystemStatus.Equals(systemStatus)
                                                                     : true) && (partnerOrderStatus != null
                                                                     ? x.PartnerOrderStatus.Equals(partnerOrderStatus)
                                                                     : true) && (searchDateFrom != null && searchDateTo != null ?
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                                     && (searchDateFrom != null && searchDateTo == null ?
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                                     && (searchDateFrom == null && searchDateTo != null ?
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true) &&
                                                                     x.OrderPartnerId.ToLower().Contains(searchName.ToLower())).CountAsync();
                }
                return await this._dbContext.Orders.Include(x => x.Store)
                                                   .ThenInclude(x => x.KitchenCenter)
                                                   .Where(x => (storeId != null
                                                                     ? x.StoreId == storeId
                                                                     : true) &&
                                                                     (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                      (systemStatus != null
                                                                     ? x.SystemStatus.Equals(systemStatus)
                                                                     : true)
                                                                     && (partnerOrderStatus != null
                                                                     ? x.PartnerOrderStatus.Equals(partnerOrderStatus)
                                                                     : true) && (searchDateFrom != null && searchDateTo != null ?
                                                                                x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                                                x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                                                && (searchDateFrom != null && searchDateTo == null ?
                                                                                x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                                                && (searchDateFrom == null && searchDateTo != null ?
                                                                                x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get orders
        public async Task<List<Order>> GetOrdersAsync(string? searchValue, string? searchValueWithoutUnicode,
                                                      int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int? storeId,
                                                      int? kitchenCenterId, string? systemStatus, string? partnerOrderStatus, string? searchDateFrom, string? searchDateTo, int? cashierId, bool? confirmedBy)
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
                else if (searchDateFrom != null && searchDateTo == null)
                {
                    startDate = DateTime.ParseExact(searchDateFrom, "dd/MM/yyyy", null);
                }
                else if (searchDateFrom == null && searchDateTo != null)
                {
                    endDate = DateTime.ParseExact(searchDateTo, "dd/MM/yyyy", null);
                }
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Orders.OrderByDescending(x => x.Id)
                                                     .Include(x => x.Store).ThenInclude(x => x.StorePartners)
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
                                                                    (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                    (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                    (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                    (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     (searchDateFrom != null && searchDateTo != null ?
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                                     && (searchDateFrom != null && searchDateTo == null ?
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                                     && (searchDateFrom == null && searchDateTo != null ?
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true))
                                                        .Where(delegate (Order order)
                                                        {
                                                            if (StringUtil.RemoveSign4VietnameseString(order.OrderPartnerId).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                            {
                                                                return true;
                                                            }
                                                            return false;
                                                        })
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("shippername"),
                                                                  then => then.OrderBy(x => x.ShipperName))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("customername"),
                                                                 then => then.OrderBy(x => x.CustomerName))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("totalstorediscount"),
                                                                         then => then.OrderBy(x => x.TotalStoreDiscount))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("finaltotalprice"),
                                                                         then => then.OrderBy(x => x.FinalTotalPrice))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("tax"),
                                                                         then => then.OrderBy(x => x.Tax))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                                                         then => then.OrderBy(x => x.Address))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                         then => then.OrderBy(x => x.OrderPartnerId))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Orders.OrderByDescending(x => x.Id)
                                                     .Include(x => x.Store).ThenInclude(x => x.StorePartners)
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
                                                                    (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                    (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                    (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                    (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) && (searchDateFrom != null && searchDateTo != null ?
                                                                               x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                                               x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                                               && (searchDateFrom != null && searchDateTo == null ?
                                                                               x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                                               && (searchDateFrom == null && searchDateTo != null ?
                                                                               x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true))
                                                        .Where(delegate (Order order)
                                                        {
                                                            if (StringUtil.RemoveSign4VietnameseString(order.OrderPartnerId).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                            {
                                                                return true;
                                                            }
                                                            return false;
                                                        })
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("shippername"),
                                                                  then => then.OrderByDescending(x => x.ShipperName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("customername"),
                                                                 then => then.OrderByDescending(x => x.CustomerName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("totalstorediscount"),
                                                                         then => then.OrderByDescending(x => x.TotalStoreDiscount))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("finaltotalprice"),
                                                                         then => then.OrderByDescending(x => x.FinalTotalPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("tax"),
                                                                         then => then.OrderByDescending(x => x.Tax))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                                                         then => then.OrderByDescending(x => x.Address))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Orders.OrderByDescending(x => x.Id)
                                                 .Include(x => x.Store).ThenInclude(x => x.StorePartners)
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
                                                        if (StringUtil.RemoveSign4VietnameseString(order.OrderPartnerId).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                        {
                                                            return true;
                                                        }
                                                        return false;
                                                    })
                                                 .Where(x => (storeId != null
                                                                   ? x.StoreId == storeId
                                                                   : true) &&
                                                                   (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                   (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                   (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                    (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) && (searchDateFrom != null && searchDateTo != null ?
                                                                               x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                                               x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                                               && (searchDateFrom != null && searchDateTo == null ?
                                                                               x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                                               && (searchDateFrom == null && searchDateTo != null ?
                                                                               x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true))
                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Orders.OrderByDescending(x => x.Id)
                                                     .Include(x => x.Store).ThenInclude(x => x.StorePartners)
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
                                                                     (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                    (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     x.OrderPartnerId.ToLower().Contains(searchValue.ToLower()) && (searchDateFrom != null && searchDateTo != null ?
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                                     && (searchDateFrom != null && searchDateTo == null ?
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                                     && (searchDateFrom == null && searchDateTo != null ?
                                                                     x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("shippername"),
                                                                      then => then.OrderBy(x => x.ShipperName))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("customername"),
                                                              then => then.OrderBy(x => x.CustomerName))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("totalstorediscount"),
                                                                      then => then.OrderBy(x => x.TotalStoreDiscount))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("finaltotalprice"),
                                                                      then => then.OrderBy(x => x.FinalTotalPrice))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("tax"),
                                                                      then => then.OrderBy(x => x.Tax))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                                                         then => then.OrderBy(x => x.Address))
                                                           .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Orders.OrderByDescending(x => x.Id)
                                                     .Include(x => x.Store).ThenInclude(x => x.StorePartners)
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
                                                                     (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                      (systemStatus != null
                                                                    ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     x.OrderPartnerId.ToLower().Contains(searchValue.ToLower()) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                      && (searchDateFrom != null && searchDateTo == null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                      && (searchDateFrom == null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("shippername"),
                                                                  then => then.OrderByDescending(x => x.ShipperName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("customername"),
                                                                 then => then.OrderByDescending(x => x.CustomerName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("totalstorediscount"),
                                                                         then => then.OrderByDescending(x => x.TotalStoreDiscount))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("finaltotalprice"),
                                                                         then => then.OrderByDescending(x => x.FinalTotalPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("tax"),
                                                                         then => then.OrderByDescending(x => x.Tax))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                                                         then => then.OrderByDescending(x => x.Address))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Orders.OrderByDescending(x => x.Id)
                                                     .Include(x => x.Store).ThenInclude(x => x.StorePartners)
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
                                                                     (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                       (systemStatus != null
                                                                     ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper())
                                                                     : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) &&
                                                                     x.OrderPartnerId.ToLower().Contains(searchValue.ToLower()) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                      && (searchDateFrom != null && searchDateTo == null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                      && (searchDateFrom == null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                if (sortByASC is not null)
                    return this._dbContext.Orders.OrderByDescending(x => x.Id)
                                                     .Include(x => x.Store).ThenInclude(x => x.StorePartners)
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
                                                                     (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) && (searchDateFrom != null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                      && (searchDateFrom != null && searchDateTo == null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                      && (searchDateFrom == null && searchDateTo != null ?
                                                      x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("shippername"),
                                                                      then => then.OrderBy(x => x.ShipperName))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("customername"),
                                                              then => then.OrderBy(x => x.CustomerName))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("totalstorediscount"),
                                                                      then => then.OrderBy(x => x.TotalStoreDiscount))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("finaltotalprice"),
                                                                      then => then.OrderBy(x => x.FinalTotalPrice))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("tax"),
                                                                      then => then.OrderBy(x => x.Tax))
                                                           .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                                                         then => then.OrderBy(x => x.Address))
                                                           .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                else if (sortByDESC is not null)
                    return this._dbContext.Orders.OrderByDescending(x => x.Id)
                                                     .Include(x => x.Store).ThenInclude(x => x.StorePartners)
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
                                                                     (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                    (partnerOrderStatus != null
                                                                    ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper())
                                                                    : true) && (searchDateFrom != null && searchDateTo != null ?
                                                                    x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                                    x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true)
                                                                    && (searchDateFrom != null && searchDateTo == null ?
                                                                    x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true)
                                                                    && (searchDateFrom == null && searchDateTo != null ?
                                                                    x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("shippername"),
                                                                  then => then.OrderByDescending(x => x.ShipperName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("customername"),
                                                                 then => then.OrderByDescending(x => x.CustomerName))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("totalstorediscount"),
                                                                         then => then.OrderByDescending(x => x.TotalStoreDiscount))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("finaltotalprice"),
                                                                         then => then.OrderByDescending(x => x.FinalTotalPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("tax"),
                                                                         then => then.OrderByDescending(x => x.Tax))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                                                         then => then.OrderByDescending(x => x.Address))
                                                           .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                return this._dbContext.Orders.OrderByDescending(x => x.Id)
                                                     .Include(x => x.Store).ThenInclude(x => x.StorePartners)
                                                     .Include(x => x.Partner)
                                                     .Include(x => x.ShipperPayments)
                                                     .Include(o => o.OrderHistories)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                                                     .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                                                     .Include(x => x.Store)
                                                     .ThenInclude(x => x.KitchenCenter)
                                                           .Where(x => (storeId != null ? x.StoreId == storeId : true) &&
                                                                     (systemStatus != null ? x.SystemStatus.ToUpper().Equals(systemStatus.Trim().ToUpper()) : true) &&
                                                                     (kitchenCenterId != null && cashierId == null ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy == null || kitchenCenterId != null && cashierId != null && confirmedBy == false ? x.Store.KitchenCenter.KitchenCenterId == kitchenCenterId : true) &&
                                                                     (kitchenCenterId != null && cashierId != null && confirmedBy != null && confirmedBy == true ? x.ConfirmedBy == cashierId : true) &&
                                                                     (partnerOrderStatus != null ? x.PartnerOrderStatus.ToUpper().Equals(partnerOrderStatus.Trim().ToUpper()) : true) &&
                                                                     (searchDateFrom != null && searchDateTo != null ?
                                                                        x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date &&
                                                                        x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true) &&
                                                                     (searchDateFrom != null && searchDateTo == null ?
                                                                        x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() >= startDate.Date : true) &&
                                                                     (searchDateFrom == null && searchDateTo != null ?
                                                                        x.OrderHistories.Select(x => x.CreatedDate.Date).SingleOrDefault() <= endDate.Date : true))
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get order by id
        public async Task<Order> GetOrderAsync(int id)
        {
            return await this._dbContext.Orders
                             .Include(x => x.Store).ThenInclude(x => x.StorePartners)
                             .Include(x => x.Store).ThenInclude(x => x.KitchenCenter)
                             .Include(x => x.Partner)
                             .Include(x => x.ShipperPayments).ThenInclude(x => x.BankingAccount)
                             .Include(x => x.OrderHistories)
                             .Include(o => o.OrderDetails).ThenInclude(x => x.MasterOrderDetail)
                             .Include(o => o.OrderDetails).ThenInclude(x => x.Product)
                             .Include(o => o.OrderDetails).ThenInclude(x => x.ExtraOrderDetails)
                             .Include(x => x.Store).SingleOrDefaultAsync(x => x.Id == id);

        }
        #endregion

        #region Get order by store id
        public async Task<List<Order>> GetOrderByStoreIdAsync(int storeId)
        {
            try
            {
                DateTime today = DateTime.Now;
                return await this._dbContext.Orders.Where(o => o.PaymentMethod.ToUpper() == OrderEnum.PaymentMethod.CASH.ToString() 
                                                       && o.StoreId == storeId
                                                       && o.ShipperPayments.Any(sp => sp.Status == (int)ShipperPaymentEnum.Status.SUCCESS
                                                                                   && sp.CreateDate.Date <= today.Date
                                                                                   && sp.CreateDate.Date >= today.AddDays(-6).Date))
                                                   .Include(o => o.ShipperPayments)
                                                   .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get order by dateFrom and dateTo
        public async Task<List<Order>> GetOrderByDateFromAndDateToAsync(DateTime? dateFrom, DateTime? dateTo, int brandId)
        {
            try
            {
                return await this._dbContext.Orders.Where(o => o.OrderHistories.Any(oh => oh.SystemStatus.ToUpper().Equals(OrderEnum.SystemStatus.COMPLETED.ToString())
                                                                                       && (dateFrom != null ? oh.CreatedDate.Date >= dateFrom.Value.Date : true)
                                                                                       && (dateTo != null ? oh.CreatedDate.Date <= dateTo.Value.Date : true))
                                                       && o.Store.Brand.BrandId == brandId)
                                                   .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                                                   .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Count number of order today by cashier id
        public async Task<int> CountNumberOfOrderTodayByCashierId(int cashierId)
        {
            try
            {
                DateTime today = DateTime.Now;
                return await this._dbContext.Orders.Where(o => o.Store.KitchenCenter.Cashiers.Any(c => c.AccountId == cashierId)
                                                       && o.OrderHistories.Any(oh => oh.CreatedDate.Date == today.Date))
                                                   .CountAsync();  

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get list order have been paid by cashier id
        public async Task<List<Order>> GetOrderPaidByDateFormAndDateToByCashierId(DateTime? dateFrom, DateTime? dateTo, int cashierId)
        {
            try
            {
                return await this._dbContext.Orders.Include(o => o.Store)
                                                   .Include(o => o.Partner)
                                                   .Include(o => o.ShipperPayments)
                                                   .Where(o => o.ShipperPayments.Any(sp => sp.CreateBy == cashierId
                                                                                 && (dateFrom != null ? sp.CreateDate.Date >= dateFrom.Value.Date : true)
                                                                                 && (dateTo != null ? sp.CreateDate.Date <= dateTo.Value.Date : true)))
                                                   .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get list of orders that have not been completed or canceled
        public async Task<List<Order>> GetOrdersOrdersNotYetProcessedToday()
        {
            try
            {
                DateTime today = DateTime.Now;
                return await this._dbContext.Orders.Include(o => o.Store)
                                                   .Where(o => (!o.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.COMPLETED.ToString().ToUpper()) && !o.PartnerOrderStatus.ToUpper().Equals(OrderEnum.Status.CANCELLED.ToString().ToUpper()))
                                                             && o.OrderHistories.Any(oh => oh.CreatedDate.Date == today.Date))
                                                   .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region update range order
        public void UpdateRangeOrder(IEnumerable<Order> orders)
        {
            try
            {
                this._dbContext.Orders.UpdateRange(orders);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
