using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class ShipperPaymentRepository
    {
        private MBKCDbContext _dbContext;
        public ShipperPaymentRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateShipperPaymentAsync(ShipperPayment shipperPayment)
        {
            try
            {
                await this._dbContext.ShipperPayments.AddAsync(shipperPayment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Get Number Shipper Payment
        public int GetNumberShipperPaymentsAsync(List<ShipperPayment> shipperPayments, string? paymentMethod, int? status, string? searchDateFrom, string? searchDateTo)
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
                return shipperPayments
                    .Where(x => (paymentMethod != null ? x.PaymentMethod.Equals(paymentMethod.ToUpper()) : true) &&
                                                 (status != null ? x.Status == status : true) &&
                                                 (searchDateFrom != null && searchDateTo != null ?
                                                 x.CreateDate.Date >= startDate.Date &&
                                                 x.CreateDate.Date <= endDate.Date : true)
                                                 && (searchDateFrom != null && searchDateTo == null ?
                                                 x.CreateDate.Date >= startDate.Date : true)
                                                 && (searchDateFrom == null && searchDateTo != null ?
                                                 x.CreateDate.Date <= endDate.Date : true)).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get shipper payments
        public List<ShipperPayment> GetShipperPaymentsAsync(List<ShipperPayment> shipperPayments,
           int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, string? paymentMethod, int? status, string? searchDateFrom, string? searchDateTo)
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
                if (sortByASC != null || sortByDESC != null)
                {
                    return (List<ShipperPayment>)shipperPayments.OrderByDescending(x => x.PaymentId).Where(x => (paymentMethod != null ? x.PaymentMethod.Equals(paymentMethod.ToUpper()) : true) &&
                                                                     (status != null ? x.Status == status : true) &&
                                                                     (searchDateFrom != null && searchDateTo != null ?
                                                                     x.CreateDate.Date >= startDate.Date &&
                                                                     x.CreateDate.Date <= endDate.Date : true)
                                                                     && (searchDateFrom != null && searchDateTo == null ?
                                                                     x.CreateDate.Date >= startDate.Date : true)
                                                                     && (searchDateFrom == null && searchDateTo != null ?
                                                                     x.CreateDate.Date <= endDate.Date : true))
                                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("amount"),
                                                                               then => then.OrderBy(x => x.Amount))
                                                                     .If(sortByASC != null && sortByASC.ToLower().Equals("createdate"),
                                                                               then => then.OrderBy(x => x.CreateDate))
                                                                     .If(sortByDESC != null && sortByDESC.ToLower().Equals("amount"),
                                                                               then => then.OrderByDescending(x => x.Amount))
                                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }

                return shipperPayments.OrderByDescending(x => x.PaymentId).Where(x => (paymentMethod != null ? x.PaymentMethod.Equals(paymentMethod.ToUpper()) : true) &&
                                                 (status != null ? x.Status == status : true) &&
                                                 (searchDateFrom != null && searchDateTo != null ?
                                                 x.CreateDate.Date >= startDate.Date &&
                                                 x.CreateDate.Date <= endDate.Date : true)
                                                 && (searchDateFrom != null && searchDateTo == null ?
                                                 x.CreateDate.Date >= startDate.Date : true)
                                                 && (searchDateFrom == null && searchDateTo != null ?
                                                 x.CreateDate.Date <= endDate.Date : true))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public async Task<List<ShipperPayment>> GetShiperPaymentsByCashierIdAsync(int cashierId)
        {
            return await this._dbContext.ShipperPayments
                .Include(x => x.Order)
                .Include(x => x.BankingAccount)
                .Where(x => x.CreateBy == cashierId).ToListAsync();
        }

        #region Count Total sales today by cashierId
        public async Task<decimal> CountTotalRevenueDailyByCashierIdAsync(int cashierId)
        {
            try
            {   DateTime today = DateTime.Now;
                return await this._dbContext.ShipperPayments.Where(sp => sp.CreateBy == cashierId
                                                                && sp.CreateDate.Date == today.Date)
                                                            .Select(sp => sp.Amount)
                                                            .SumAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get shipper payment of today by cashierId
        public async Task<List<ShipperPayment>> GetFiveShiperPaymentsSoryByCreatDateFindByCashierIdAsync(int cashierId)
        {
            try
            {
                return await this._dbContext.ShipperPayments.Include(x => x.BankingAccount).Include(sp => sp.Order)
                                                            .Where(sp => sp.CreateBy == cashierId)
                                                            .OrderByDescending(sp => sp.CreateDate)
                                                            .Take(5)
                                                            .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
