using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
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
                if (sortByASC != null && sortByDESC != null)

                    return moneyExchanges.Where(x => (exchangeType != null ? x.ExchangeType.Equals(exchangeType.ToUpper()) : true) &&
                                                     (status != null ? x.Status == status : true) &&
                                                     (searchDateFrom != null && searchDateTo != null ? x.Transactions.Any(o => o.TransactionTime >= startDate && o.TransactionTime <= endDate) : true))
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return (List<MoneyExchange>)moneyExchanges.Where(x => (exchangeType != null ? x.ExchangeType.Equals(exchangeType.ToUpper()) : true) &&
                                                 (status != null ? x.Status == status : true) &&
                                                 (searchDateFrom != null && searchDateTo != null ? x.Transactions.Any(o => o.TransactionTime >= startDate && o.TransactionTime <= endDate) : true))
                                                 .If(sortByASC != null && sortByASC.ToLower().Equals("amount"),
                                                           then => then.OrderBy(x => x.Amount))
                                                 .If(sortByDESC != null && sortByDESC.ToLower().Equals("amount"),
                                                           then => then.OrderByDescending(x => x.Amount))
                                                 .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number Brands
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
                return moneyExchanges
                    .Where(x => (exchangeType != null ? x.ExchangeType.Equals(exchangeType.ToUpper()) : true) &&
                                                 (status != null ? x.Status == status : true) &&
                                                 (searchDateFrom != null && searchDateTo != null ? x.Transactions.Any(o => o.TransactionTime >= startDate && o.TransactionTime <= endDate) : true)).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
