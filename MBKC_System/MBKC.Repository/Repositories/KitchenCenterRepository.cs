using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;

namespace MBKC.Repository.Repositories
{
    public class KitchenCenterRepository
    {
        private MBKCDbContext _dbContext;
        public KitchenCenterRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateKitchenCenterAsync(KitchenCenter kitchenCenter)
        {
            try
            {
                await this._dbContext.KitchenCenters.AddAsync(kitchenCenter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateKitchenCenter(KitchenCenter kitchenCenter)
        {
            try
            {
                this._dbContext.KitchenCenters.Update(kitchenCenter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<KitchenCenter> GetKitchenCenterAsync(int id)
        {
            try
            {
                return await this._dbContext.KitchenCenters.Include(x => x.Manager)
                                                           .Include(x => x.Stores).ThenInclude(x => x.Brand)
                                                           .Include(x => x.KitchenCenterMoneyExchanges)
                                                           .ThenInclude(x => x.MoneyExchange)
                                                           .ThenInclude(x => x.Transactions)
                                                           .ThenInclude(x => x.Wallet)
                                                           .Include(x => x.KitchenCenterMoneyExchanges)
                                                           .ThenInclude(x => x.MoneyExchange)
                                                           .ThenInclude(x => x.Transactions)
                                                           .ThenInclude(x => x.ShipperPayment).ThenInclude(x => x.Order)
                                                           .Include(x => x.KitchenCenterMoneyExchanges)
                                                           .ThenInclude(x => x.MoneyExchange)
                                                           .ThenInclude(x => x.Transactions)
                                                           .ThenInclude(x => x.ShipperPayment).ThenInclude(x => x.BankingAccount)
                                                           .FirstOrDefaultAsync(x => x.KitchenCenterId == id && x.Status != (int)KitchenCenterEnum.Status.DEACTIVE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetNumberKitchenCentersAsync(string? searchValue, string? searchValueWithoutUnicode)
        {
            try
            {
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.KitchenCenters.Where(delegate (KitchenCenter kitchenCenter)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(kitchenCenter.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(x => x.Status != (int)KitchenCenterEnum.Status.DEACTIVE).AsQueryable().Count();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.KitchenCenters.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                           x.Status != (int)KitchenCenterEnum.Status.DEACTIVE).CountAsync();
                }
                return await this._dbContext.KitchenCenters.Where(x => x.Status != (int)KitchenCenterEnum.Status.DEACTIVE).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<KitchenCenter>> GetKitchenCentersAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, bool? isGetAll)
        {
            try
            {
                if (isGetAll != null && isGetAll == true)
                {
                    return this._dbContext.KitchenCenters.Include(x => x.Manager)
                   .Where(x => x.Status != (int)KitchenCenterEnum.Status.DEACTIVE).ToList();
                }
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.KitchenCenters.Include(x => x.Manager).Where(delegate (KitchenCenter kitchenCenter)
                        {
                            if (StringUtil.RemoveSign4VietnameseString(kitchenCenter.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        })
                        .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                 then => then.OrderBy(x => x.Name))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                 then => then.OrderBy(x => x.Address))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                 then => then.OrderBy(x => x.Status).Reverse())
                        .If(sortByASC != null && sortByASC.ToLower().Equals("kitchencentermanageremail"),
                                 then => then.OrderBy(x => x.Manager.Email))
                        .Where(x => x.Status != (int)KitchenCenterEnum.Status.DEACTIVE).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.KitchenCenters.Include(x => x.Manager).Where(delegate (KitchenCenter kitchenCenter)
                        {
                            if (StringUtil.RemoveSign4VietnameseString(kitchenCenter.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        })
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                 then => then.OrderByDescending(x => x.Name))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                 then => then.OrderByDescending(x => x.Address))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                 then => then.OrderByDescending(x => x.Status).Reverse())
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("kitchencentermanageremail"),
                                 then => then.OrderByDescending(x => x.Manager.Email))
                        .Where(x => x.Status != (int)KitchenCenterEnum.Status.DEACTIVE).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.KitchenCenters.Include(x => x.Manager).Where(delegate (KitchenCenter kitchenCenter)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(kitchenCenter.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(x => x.Status != (int)KitchenCenterEnum.Status.DEACTIVE).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.KitchenCenters.Include(x => x.Manager)
                        .Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) && x.Status != (int)KitchenCenterEnum.Status.DEACTIVE)
                        .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                 then => then.OrderBy(x => x.Name))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                 then => then.OrderBy(x => x.Address))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                 then => then.OrderBy(x => x.Status).Reverse())
                        .If(sortByASC != null && sortByASC.ToLower().Equals("kitchencentermanageremail"),
                                 then => then.OrderBy(x => x.Manager.Email))
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.KitchenCenters.Include(x => x.Manager)
                       .Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) && x.Status != (int)KitchenCenterEnum.Status.DEACTIVE)
                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                 then => then.OrderByDescending(x => x.Name))
                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                                 then => then.OrderByDescending(x => x.Address))
                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                 then => then.OrderByDescending(x => x.Status).Reverse())
                       .If(sortByDESC != null && sortByDESC.ToLower().Equals("kitchencentermanageremail"),
                                 then => then.OrderByDescending(x => x.Manager.Email))
                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.KitchenCenters.Include(x => x.Manager)
                       .Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) && x.Status != (int)KitchenCenterEnum.Status.DEACTIVE)
                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                if (sortByASC is not null)
                    return this._dbContext.KitchenCenters.Include(x => x.Manager)
                    .Where(x => x.Status != (int)KitchenCenterEnum.Status.DEACTIVE)
                    .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                 then => then.OrderBy(x => x.Name))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("address"),
                                 then => then.OrderBy(x => x.Address))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                 then => then.OrderBy(x => x.Status).Reverse())
                        .If(sortByASC != null && sortByASC.ToLower().Equals("kitchencentermanageremail"),
                                 then => then.OrderBy(x => x.Manager.Email))
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                else if (sortByDESC is not null)
                    return this._dbContext.KitchenCenters.Include(x => x.Manager)
                   .Where(x => x.Status != (int)KitchenCenterEnum.Status.DEACTIVE)
                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                              then => then.OrderByDescending(x => x.Name))
                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("address"),
                              then => then.OrderByDescending(x => x.Address))
                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                              then => then.OrderByDescending(x => x.Status).Reverse())
                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("kitchencentermanageremail"),
                              then => then.OrderByDescending(x => x.Manager.Email))
                   .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return await this._dbContext.KitchenCenters.Include(x => x.Manager)
                  .Where(x => x.Status != (int)KitchenCenterEnum.Status.DEACTIVE)
                  .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<KitchenCenter> GetKitchenCenterAsync(string managerEmail)
        {
            try
            {
                return await this._dbContext.KitchenCenters.Include(x => x.Manager)
                                                           .Include(x => x.KitchenCenterMoneyExchanges)
                                                           .Include(x => x.Cashiers)
                                                           .Include(x => x.Stores)
                                                           .Include(x => x.Wallet)
                                                           .ThenInclude(x => x.Transactions)
                                                           .ThenInclude(x => x.MoneyExchange)
                                                           .Include(x => x.Wallet)
                                                           .ThenInclude(x => x.Transactions)
                                                           .ThenInclude(x => x.ShipperPayment)
                                                           .Include(x => x.Wallet)
                                                           .ThenInclude(x => x.Transactions)
                                                           .ThenInclude(x => x.ShipperPayment)
                                                           .ThenInclude(x => x.Order)
                                                           .Include(x => x.BankingAccounts).ThenInclude(x => x.ShipperPayments)
                                                           .FirstOrDefaultAsync(x => x.Manager.Email.Equals(managerEmail)
                                                                                  && x.Status != (int)KitchenCenterEnum.Status.DEACTIVE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<KitchenCenter>> GetKitchenCentersIncludeOrderAsync()
        {
            try
            {
                return await this._dbContext.KitchenCenters.Include(kc => kc.Manager)
                                                           .Include(kc => kc.Wallet)
                                                           .Include(kc => kc.KitchenCenterMoneyExchanges.Where(kc => kc.MoneyExchange.ExchangeType.ToUpper().Equals(MoneyExchangeEnum.ExchangeType.SEND.ToString())
                                                                                                                  && kc.MoneyExchange.Transactions.Any(ts => ts.TransactionTime.Day == DateTime.Now.Day
                                                                                                                                                          && ts.TransactionTime.Month == DateTime.Now.Month
                                                                                                                                                          && ts.TransactionTime.Year == DateTime.Now.Year)))
                                                           .Include(kc => kc.Stores.Where(s => s.Status == (int)StoreEnum.Status.ACTIVE))
                                                           .ThenInclude(s => s.Orders.Where(o => o.PartnerOrderStatus.Equals(OrderEnum.Status.COMPLETED.ToString())
                                                                                              && o.PaymentMethod.ToUpper().Equals(OrderEnum.PaymentMethod.CASH.ToString())
                                                                                              && o.ShipperPayments.Any(sp => sp.CreateDate.Day == DateTime.Now.Day
                                                                                                                          && sp.CreateDate.Month == DateTime.Now.Month
                                                                                                                          && sp.CreateDate.Year == DateTime.Now.Year)))
                                                           .Include(kc => kc.Stores)
                                                           .ThenInclude(s => s.Wallet)
                                                           .Where(kc => kc.Status == (int)KitchenCenterEnum.Status.ACTIVE && kc.Wallet.Balance > 0).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
