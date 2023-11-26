using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
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
    public class MoneyExchangeRepository
    {
        private MBKCDbContext _dbContext;
        public MoneyExchangeRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region Get
        //public async Task<List<MoneyExchange>> GetMoneyExchangesAsync()
        //{

        //}

        //public async Task<int> GetNumberOrdersAsync()
        //{

        //}

        #endregion

        #region Insert
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

        public async Task CreateRangeMoneyExchangeAsync(IEnumerable<MoneyExchange> moneyExchanges)
        {
            try
            {
                await this._dbContext.MoneyExchanges.AddRangeAsync(moneyExchanges);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get money exchanges
        public List<MoneyExchange> GetMoneyExchangesAsync(List<MoneyExchange> moneyExchanges,
           int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, string? exchangeType, int? status, string? searchDateFrom, string? searchDateTo)
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
                    return moneyExchanges.OrderByDescending(x => x.ExchangeId).Where(x => (exchangeType != null ? x.ExchangeType.Equals(exchangeType.ToUpper()) : true) &&
                                                                    (status != null ? x.Status == status : true) &&
                                                                    (searchDateFrom != null && searchDateTo != null ?
                                                                    x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date &&
                                                                    x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true)
                                                                    && (searchDateFrom != null && searchDateTo == null ?
                                                                    x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date : true)
                                                                    && (searchDateFrom == null && searchDateTo != null ?
                                                                    x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true))
                                                                    .If(sortByASC != null && sortByASC.ToLower().Equals("amount"),
                                                                              then => then.OrderBy(x => x.Amount))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("amount"),
                                                                              then => then.OrderByDescending(x => x.Amount))
                                                                    .If(sortByASC != null && sortByASC.ToLower().Equals("transactiontime"),
                                                                              then => then.OrderBy(x => x.Transactions.Select(x => x.TransactionTime))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("transactiontime"),
                                                                              then => then.OrderByDescending(x => x.Transactions.Select(x => x.TransactionTime)))
                                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)).ToList();
                }

                return moneyExchanges.OrderByDescending(x => x.ExchangeId).Where(x => (exchangeType != null ? x.ExchangeType.Equals(exchangeType.ToUpper()) : true) &&
                                                     (status != null ? x.Status == status : true) &&
                                                     (searchDateFrom != null && searchDateTo != null ?
                                                     x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date &&
                                                     x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true)
                                                     && (searchDateFrom != null && searchDateTo == null ?
                                                     x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date : true)
                                                     && (searchDateFrom == null && searchDateTo != null ?
                                                     x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true))
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number Money Exchanges
        public int GetNumberMoneyExchangesAsync(List<MoneyExchange> moneyExchanges, string? exchangeType, int? status, string? searchDateFrom, string? searchDateTo)
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
                return moneyExchanges
                    .Where(x => (exchangeType != null ? x.ExchangeType.Equals(exchangeType.ToUpper()) : true) &&
                                                 (status != null ? x.Status == status : true) &&
                                                 (searchDateFrom != null && searchDateTo != null ?
                                                  x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date &&
                                                  x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true)
                                                  && (searchDateFrom != null && searchDateTo == null ?
                                                  x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date : true)
                                                  && (searchDateFrom == null && searchDateTo != null ?
                                                  x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true)).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Money Exchanges By Sender Id
        public async Task<List<MoneyExchange>> GetMoneyExchangesBySendIdAsync(int senderId)
        {
            return await this._dbContext.MoneyExchanges
                .Include(x => x.Transactions)
                .Where(x => x.SenderId == senderId && x.ExchangeType.Equals(MoneyExchangeEnum.ExchangeType.WITHDRAW.ToString())).ToListAsync();
        }
        #endregion

        #region Get money exchange in last 7 day for kitchen center (receive)
        public async Task<List<MoneyExchange>> GetColumnChartMoneyExchangeInLastSevenDayAsync(int kitchenCenterId)
        {
            try
            {
                DateTime today = DateTime.Now;
                return await _dbContext.MoneyExchanges.Include(me => me.Transactions)
                                                      .Where(me => me.ExchangeType.ToUpper().Equals(MoneyExchangeEnum.ExchangeType.RECEIVE.ToString())
                                                          && me.ReceiveId == kitchenCenterId
                                                          && me.Transactions.Any(t => t.Status == (int)TransactionEnum.Status.SUCCESS
                                                                              && t.TransactionTime.Date <= today.Date
                                                                              && t.TransactionTime.Date >= today.AddDays(-6).Date))
                                                      .ToListAsync();
                                                     
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number Money Exchanges With Draw
        public int GetNumberMoneyExchangesWithDrawAsync(List<MoneyExchange> moneyExchanges, int? status, string? searchDateFrom, string? searchDateTo)
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
                return moneyExchanges
                    .Where(x => (status != null ? x.Status == status : true) &&
                                                 (searchDateFrom != null && searchDateTo != null ?
                                                  x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date &&
                                                  x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true)
                                                  && (searchDateFrom != null && searchDateTo == null ?
                                                  x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date : true)
                                                  && (searchDateFrom == null && searchDateTo != null ?
                                                  x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true)).Count();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get money exchanges with draw
        public List<MoneyExchange> GetMoneyExchangesWithDrawAsync(List<MoneyExchange> moneyExchanges,
           int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int? status, string? searchDateFrom, string? searchDateTo)
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
                    return moneyExchanges.OrderByDescending(x => x.ExchangeId).Where(x => (status != null ? x.Status == status : true) &&
                                                                    (searchDateFrom != null && searchDateTo != null ?
                                                                    x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date &&
                                                                    x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true)
                                                                    && (searchDateFrom != null && searchDateTo == null ?
                                                                    x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date : true)
                                                                    && (searchDateFrom == null && searchDateTo != null ?
                                                                    x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true))
                                                                    .If(sortByASC != null && sortByASC.ToLower().Equals("amount"),
                                                                              then => then.OrderBy(x => x.Amount))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("amount"),
                                                                              then => then.OrderByDescending(x => x.Amount))
                                                                    .If(sortByASC != null && sortByASC.ToLower().Equals("transactiontime"),
                                                                              then => then.OrderBy(x => x.Transactions.Select(x => x.TransactionTime))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("transactiontime"),
                                                                              then => then.OrderByDescending(x => x.Transactions.Select(x => x.TransactionTime)))
                                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)).ToList();

                }

                return moneyExchanges.OrderByDescending(x => x.ExchangeId).Where(x => (status != null ? x.Status == status : true) &&
                                                     (searchDateFrom != null && searchDateTo != null ?
                                                     x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date &&
                                                     x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true)
                                                     && (searchDateFrom != null && searchDateTo == null ?
                                                     x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() >= startDate.Date : true)
                                                     && (searchDateFrom == null && searchDateTo != null ?
                                                     x.Transactions.Select(x => x.TransactionTime.Date).SingleOrDefault() <= endDate.Date : true))
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

    }
}

