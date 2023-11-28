using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;

namespace MBKC.Repository.Repositories
{
    public class CashierRepository
    {
        private MBKCDbContext _dbContext;
        public CashierRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<Cashier>> GetCashiersAsync()
        {
            try
            {
                return await this._dbContext.Cashiers.Include(c => c.Wallet)
                                                     .Include(c => c.KitchenCenter).ThenInclude(kc => kc.Wallet)
                                                     .Include(c => c.CashierMoneyExchanges.Where(ce => ce.MoneyExchange.ExchangeType.ToUpper().Equals(MoneyExchangeEnum.ExchangeType.SEND.ToString())
                                                                                              && ce.MoneyExchange.Transactions.Any(ts => ts.TransactionTime.Day == DateTime.Now.Day
                                                                                                                                      && ts.TransactionTime.Month == DateTime.Now.Month
                                                                                                                                      && ts.TransactionTime.Year == DateTime.Now.Year)))
                                                     .Where(c => c.Account.Status == (int)AccountEnum.Status.ACTIVE
                                                              && c.Wallet.Balance > 0).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cashier> GetCashiersIncludeMoneyExchangeAsync(string email)
        {
            try
            {
                var currentDate = DateTime.Now.Date;
                return await this._dbContext.Cashiers.Include(c => c.Wallet)
                                                     .Include(c => c.KitchenCenter).ThenInclude(kc => kc.Wallet)
                                                     .Include(c => c.CashierMoneyExchanges.Where(ce => ce.MoneyExchange.ExchangeType.ToUpper().Equals(MoneyExchangeEnum.ExchangeType.SEND.ToString())
                                                                                              && ce.MoneyExchange.Transactions.Any(ts => ts.TransactionTime.Date == currentDate)))

                                                     .SingleOrDefaultAsync(c => c.Account.Email.Equals(email)
                                                                             && c.Account.Status == (int)AccountEnum.Status.ACTIVE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Cashier>> GetCashiersAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int kitchenCenterId)
        {
            try
            {
                if (searchValue is null && searchValueWithoutUnicode is not null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE)
                                                             .Where(delegate (Cashier cashier)
                                                             {
                                                                 if (StringUtil.RemoveSign4VietnameseString(cashier.FullName).ToLower().Contains(searchValueWithoutUnicode.ToLower())) return true;
                                                                 return false;
                                                             })
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("fullname"),
                                                                  then => then.OrderBy(x => x.FullName))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("gender"),
                                                                  then => then.OrderBy(x => x.Gender))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("dateofbirth"),
                                                                  then => then.OrderBy(x => x.DateOfBirth))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("citizennumber"),
                                                                  then => then.OrderBy(x => x.CitizenNumber))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("email"),
                                                                  then => then.OrderBy(x => x.Account.Email))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Account.Status))
                                                             .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                             .ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE)
                                                             .Where(delegate (Cashier cashier)
                                                             {
                                                                 if (StringUtil.RemoveSign4VietnameseString(cashier.FullName).ToLower().Contains(searchValueWithoutUnicode.ToLower())) return true;
                                                                 return false;
                                                             })
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("fullname"),
                                                                  then => then.OrderByDescending(x => x.FullName))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("gender"),
                                                                  then => then.OrderByDescending(x => x.Gender))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("dateofbirth"),
                                                                  then => then.OrderByDescending(x => x.DateOfBirth))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("citizennumber"),
                                                                  then => then.OrderByDescending(x => x.CitizenNumber))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("email"),
                                                                  then => then.OrderByDescending(x => x.Account.Email))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                  then => then.OrderByDescending(x => x.Account.Status))
                                                             .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                             .ToList();

                    return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                         .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE)
                                                         .Where(delegate (Cashier cashier)
                                                         {
                                                             if (StringUtil.RemoveSign4VietnameseString(cashier.FullName).ToLower().Contains(searchValueWithoutUnicode.ToLower())) return true;
                                                             return false;
                                                         })
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue is not null && searchValueWithoutUnicode is null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE && x.FullName.ToLower().Contains(searchValue.ToLower()))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("fullname"),
                                                                  then => then.OrderBy(x => x.FullName))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("gender"),
                                                                  then => then.OrderBy(x => x.Gender))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("dateofbirth"),
                                                                  then => then.OrderBy(x => x.DateOfBirth))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("citizennumber"),
                                                                  then => then.OrderBy(x => x.CitizenNumber))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("email"),
                                                                  then => then.OrderBy(x => x.Account.Email))
                                                             .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Account.Status))
                                                             .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                             .ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE && x.FullName.ToLower().Contains(searchValue.ToLower()))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("fullname"),
                                                                  then => then.OrderByDescending(x => x.FullName))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("gender"),
                                                                  then => then.OrderByDescending(x => x.Gender))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("dateofbirth"),
                                                                  then => then.OrderByDescending(x => x.DateOfBirth))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("citizennumber"),
                                                                  then => then.OrderByDescending(x => x.CitizenNumber))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("email"),
                                                                  then => then.OrderByDescending(x => x.Account.Email))
                                                             .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                  then => then.OrderByDescending(x => x.Account.Status))
                                                             .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                             .ToList();

                    return await this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                         .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE && x.FullName.ToLower().Contains(searchValue.ToLower()))
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
                }

                if (sortByASC is not null)
                    return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                         .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE)
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("fullname"),
                                                              then => then.OrderBy(x => x.FullName))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("gender"),
                                                              then => then.OrderBy(x => x.Gender))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("dateofbirth"),
                                                              then => then.OrderBy(x => x.DateOfBirth))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("citizennumber"),
                                                              then => then.OrderBy(x => x.CitizenNumber))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("email"),
                                                              then => then.OrderBy(x => x.Account.Email))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                              then => then.OrderBy(x => x.Account.Status))
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                         .ToList();
                else if (sortByDESC is not null)
                    return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                         .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE)
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("fullname"),
                                                              then => then.OrderByDescending(x => x.FullName))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("gender"),
                                                              then => then.OrderByDescending(x => x.Gender))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("dateofbirth"),
                                                              then => then.OrderByDescending(x => x.DateOfBirth))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("citizennumber"),
                                                              then => then.OrderByDescending(x => x.CitizenNumber))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("email"),
                                                              then => then.OrderByDescending(x => x.Account.Email))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                              then => then.OrderByDescending(x => x.Account.Status))
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                                                         .ToList();

                return await this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE)
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<int> GetNumberCashiersAsync(string? searchValue, string? searchValueWithoutUnicode, int kitchenCenterId)
        {
            try
            {
                if (searchValue is not null && searchValueWithoutUnicode is null)
                {
                    return await this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE && x.FullName.ToLower().Contains(searchValue.ToLower()))
                                                             .CountAsync();
                }
                else if (searchValue is null && searchValueWithoutUnicode is not null)
                {
                    return this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                             .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE)
                                                             .Where(delegate (Cashier cashier)
                                                             {
                                                                 if (StringUtil.RemoveSign4VietnameseString(cashier.FullName).ToLower().Contains(searchValueWithoutUnicode.ToLower())) return true;
                                                                 return false;
                                                             }).Count();
                }
                return await this._dbContext.Cashiers.Include(x => x.Account).Include(x => x.KitchenCenter)
                                                     .Where(x => x.KitchenCenter.KitchenCenterId == kitchenCenterId && x.Account.Status != (int)AccountEnum.Status.DISABLE)
                                                     .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cashier> GetCashierAsync(string email)
        {
            try
            {
                var currentDate = DateTime.Now.Date;
                return await this._dbContext.Cashiers.Include(x => x.Account)
                                                     .Include(x => x.Wallet)
                                                     .Include(x => x.Wallet)
                                                     .Include(x => x.KitchenCenter).ThenInclude(kc => kc.Manager)
                                                     .Include(x => x.KitchenCenter).ThenInclude(kc => kc.Stores)
                                                     .Include(x => x.KitchenCenter).ThenInclude(kc => kc.BankingAccounts)
                                                     .Include(x => x.KitchenCenter).ThenInclude(kc => kc.Wallet)
                                                     .Include(x => x.CashierMoneyExchanges.Where(x => x.MoneyExchange.ExchangeType.ToUpper().Equals(MoneyExchangeEnum.ExchangeType.SEND.ToString())
                                                                                                                  && x.MoneyExchange.Transactions.Any(ts => ts.TransactionTime.Date == currentDate)))

                                                     .SingleOrDefaultAsync(x => x.Account.Email.Equals(email));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cashier> GetCashierWithCitizenNumberAsync(string citizenNumber)
        {
            try
            {
                return await this._dbContext.Cashiers.SingleOrDefaultAsync(x => x.CitizenNumber.Equals(citizenNumber));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateCashierAsync(Cashier cashier)
        {
            try
            {
                await this._dbContext.Cashiers.AddAsync(cashier);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cashier> GetCashierAsync(int idCashier)
        {
            try
            {
                return await this._dbContext.Cashiers.Include(x => x.Account)
                                                     .Include(x => x.CashierMoneyExchanges)
                                                     .Include(x => x.KitchenCenter)
                                                     .Include(x => x.Wallet).ThenInclude(x => x.Transactions).ThenInclude(x => x.MoneyExchange)
                                                     .Include(x => x.Wallet).ThenInclude(x => x.Transactions).ThenInclude(x => x.ShipperPayment).ThenInclude(x => x.Order)
                                                     .Include(x => x.Wallet).ThenInclude(x => x.Transactions).ThenInclude(x => x.ShipperPayment).ThenInclude(x => x.BankingAccount)
                                                     .SingleOrDefaultAsync(x => x.AccountId == idCashier && x.Account.Status != (int)AccountEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cashier> GetCashierWalletAsync(int idCashier)
        {
            try
            {
                return await this._dbContext.Cashiers.Include(x => x.Wallet)
                                                     .Include(x => x.CashierMoneyExchanges).ThenInclude(x => x.MoneyExchange).ThenInclude(x => x.Transactions)
                                                     .Include(x => x.KitchenCenter).ThenInclude(x => x.BankingAccounts).ThenInclude(x => x.ShipperPayments)
                                                     .SingleOrDefaultAsync(x => x.AccountId == idCashier && x.Account.Status != (int)AccountEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateCashierAsync(Cashier cashier)
        {
            try
            {
                this._dbContext.Cashiers.Update(cashier);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cashier> GetCashierWithMoneyExchangeTypeIsSendAsync(int idCashier)
        {
            try
            {
                var currentDate = DateTime.Now.Date;
                return await this._dbContext.Cashiers.Include(x => x.Account)
                                                     .Include(x => x.KitchenCenter)
                                                     .Include(x => x.CashierMoneyExchanges.Where(x => x.MoneyExchange.ExchangeType.ToUpper().Equals(MoneyExchangeEnum.ExchangeType.SEND.ToString())
                                                                                                                  && x.MoneyExchange.Transactions.Any(ts => ts.TransactionTime.Date == currentDate)))
                                                     .SingleOrDefaultAsync(x => x.AccountId == idCashier && x.Account.Status != (int)AccountEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cashier> GetCashierMoneyExchangeAsync(string email)
        {
            try
            {
                return await this._dbContext.Cashiers.Include(x => x.KitchenCenter)
                                                     .Include(x => x.CashierMoneyExchanges).ThenInclude(x => x.MoneyExchange).ThenInclude(x => x.Transactions)
                                                     .SingleOrDefaultAsync(x => x.Account.Email.Equals(email));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cashier> GetCashierShipperPaymentAsync(string email)
        {
            try
            {
                return await this._dbContext.Cashiers.SingleOrDefaultAsync(x => x.Account.Email.Equals(email));


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cashier> GetCashierReportAsync(string email)
        {
            try
            {
                return await this._dbContext.Cashiers.Include(x => x.CashierMoneyExchanges).ThenInclude(x => x.MoneyExchange).ThenInclude(x => x.Transactions)
                                                     .Include(x => x.Account)
                                                     .Include(x => x.Wallet)
                                                     .Include(x => x.KitchenCenter).ThenInclude(kc => kc.Manager)
                                                     .Include(x => x.KitchenCenter).ThenInclude(kc => kc.Stores).ThenInclude(x => x.Orders).ThenInclude(x => x.OrderHistories)
                                                     .Include(x => x.KitchenCenter).ThenInclude(kc => kc.Stores).ThenInclude(x => x.Orders).ThenInclude(x => x.ShipperPayments)
                                                     .SingleOrDefaultAsync(x => x.Account.Email.Equals(email));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region count cashier in system
        public async Task<int> CountCashierInSystemFindByKitchenCenterIdAsync(int kitchenCenterId)
        {
            try
            {
                return await this._dbContext.Cashiers.Where(c => c.Account.Status != (int)AccountEnum.Status.DISABLE && c.KitchenCenter.KitchenCenterId == kitchenCenterId).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region get cashier for dashboard
        public async Task<Cashier?> GetCashierForDashBoardAsync(string email)
        {
            try
            {
                return await this._dbContext.Cashiers.Include(c => c.CashierMoneyExchanges.OrderByDescending(cm => cm.ExchangeId).Take(5))
                                                     .ThenInclude(cm => cm.MoneyExchange).ThenInclude(me => me.Transactions)
                                                     .SingleOrDefaultAsync(x => x.Account.Email.Equals(email));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
