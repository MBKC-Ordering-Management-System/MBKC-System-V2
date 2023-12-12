using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;

namespace MBKC.Repository.Repositories
{
    public class BankingAccountRepository
    {
        private MBKCDbContext _dbContext;
        public BankingAccountRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<int> GetNumberBankingAccountsAsync(int kitchenCenterId, string? searchValue, string? searchValueWithoutUnicode)
        {
            try
            {
                if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.BankingAccounts.Where(x => x.KitchenCenterId == kitchenCenterId
                                                                         && x.Name.ToLower().Contains(searchValue.ToLower())
                                                                         && x.Status != (int)BankingAccountEnum.Status.DISABLE).CountAsync();
                }
                else if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.BankingAccounts.Where(x => x.KitchenCenterId == kitchenCenterId && x.Status != (int)BankingAccountEnum.Status.DISABLE).Where(delegate (BankingAccount bankingAccount)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(bankingAccount.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        return false;
                    }).AsQueryable().Count();
                }
                return await this._dbContext.BankingAccounts.Where(x => x.KitchenCenterId == kitchenCenterId && x.Status != (int)BankingAccountEnum.Status.DISABLE).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BankingAccount>> GetBankingAccountsAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int? kitchenCenterId, bool? isGetAll)
        {
            try
            {
                if (isGetAll is not null && isGetAll == true)
                {
                    return this._dbContext.BankingAccounts
                       .Include(x => x.KitchenCenter)
                       .Where(x => x.KitchenCenterId == kitchenCenterId && x.Status != (int)BankingAccountEnum.Status.DISABLE)
                       .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.BankingAccounts
                                                                    .Include(x => x.KitchenCenter)
                                                                    .Where(x => x.KitchenCenterId == kitchenCenterId
                                                                             && x.Status != (int)BankingAccountEnum.Status.DISABLE)
                                                                    .Where(delegate (BankingAccount bankingAccount)
                                                                    {
                                                                        if (StringUtil.RemoveSign4VietnameseString(bankingAccount.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                                        {
                                                                            return true;
                                                                        }
                                                                        return false;
                                                                    })
                                                                    .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                             then => then.OrderBy(x => x.Name))
                                                                    .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                             then => then.OrderBy(x => x.Status).Reverse())
                                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.BankingAccounts
                                                                   .Include(x => x.KitchenCenter)
                                                                   .Where(x => x.KitchenCenterId == kitchenCenterId
                                                                            && x.Status != (int)BankingAccountEnum.Status.DISABLE)
                                                                   .Where(delegate (BankingAccount bankingAccount)
                                                                   {
                                                                       if (StringUtil.RemoveSign4VietnameseString(bankingAccount.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                                       {
                                                                           return true;
                                                                       }
                                                                       return false;
                                                                   })
                                                                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                            then => then.OrderByDescending(x => x.Name))
                                                                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                            then => then.OrderByDescending(x => x.Status).Reverse())
                                                                   .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.BankingAccounts
                                                                   .Include(x => x.KitchenCenter)
                                                                   .Where(x => x.KitchenCenterId == kitchenCenterId
                                                                            && x.Status != (int)BankingAccountEnum.Status.DISABLE)
                                                                   .Where(delegate (BankingAccount bankingAccount)
                                                                   {
                                                                       if (StringUtil.RemoveSign4VietnameseString(bankingAccount.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                                       {
                                                                           return true;
                                                                       }
                                                                       return false;
                                                                   }).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.BankingAccounts
                                                                    .Include(x => x.KitchenCenter)
                                                                    .Where(x => x.KitchenCenterId == kitchenCenterId
                                                                        && x.Status != (int)BankingAccountEnum.Status.DISABLE
                                                                        && x.Name.ToLower().Contains(searchValue.ToLower()))
                                                                    .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                             then => then.OrderBy(x => x.Name))
                                                                    .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                             then => then.OrderBy(x => x.Status).Reverse())
                                                               .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.BankingAccounts
                                                                    .Include(x => x.KitchenCenter)
                                                                    .Where(x => x.KitchenCenterId == kitchenCenterId
                                                                        && x.Status != (int)BankingAccountEnum.Status.DISABLE
                                                                        && x.Name.ToLower().Contains(searchValue.ToLower()))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                             then => then.OrderByDescending(x => x.Name))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                             then => then.OrderByDescending(x => x.Status).Reverse())
                                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                              return this._dbContext.BankingAccounts
                                                                    .Include(x => x.KitchenCenter)
                                                                    .Where(x => x.KitchenCenterId == kitchenCenterId
                                                                        && x.Status != (int)BankingAccountEnum.Status.DISABLE
                                                                        && x.Name.ToLower().Contains(searchValue.ToLower()))
                                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }

                if (sortByASC is not null)
                    return this._dbContext.BankingAccounts
                                                          .Include(x => x.KitchenCenter)
                                                          .Where(x => x.KitchenCenterId == kitchenCenterId && x.Status != (int)BankingAccountEnum.Status.DISABLE)
                                                                .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                             then => then.OrderBy(x => x.Name))
                                                                .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                             then => then.OrderBy(x => x.Status).Reverse())
                                                                .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                else if (sortByDESC is not null)
                    return this._dbContext.BankingAccounts
                                                          .Include(x => x.KitchenCenter)
                                                          .Where(x => x.KitchenCenterId == kitchenCenterId && x.Status != (int)BankingAccountEnum.Status.DISABLE)
                                                                .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                             then => then.OrderByDescending(x => x.Name))
                                                                .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                             then => then.OrderByDescending(x => x.Status).Reverse())
                                                                .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return this._dbContext.BankingAccounts
                                                      .Include(x => x.KitchenCenter)
                                                      .Where(x => x.KitchenCenterId == kitchenCenterId && x.Status != (int)BankingAccountEnum.Status.DISABLE)
                                                      .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BankingAccount> GetBankingAccountAsync(int bankingAccountId)
        {
            try
            {
                return await this._dbContext.BankingAccounts.FirstOrDefaultAsync(x => x.BankingAccountId == bankingAccountId && x.Status != (int)BankingAccountEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BankingAccount> GetBankingAccountAsync(string numberAccount)
        {
            try
            {
                return await this._dbContext.BankingAccounts.FirstOrDefaultAsync(x => x.NumberAccount.Equals(numberAccount) && x.Status !=  (int)BankingAccountEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateBankingAccountAsync(BankingAccount bankingAccount)
        {
            try
            {
                await this._dbContext.BankingAccounts.AddAsync(bankingAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateBankingAccount(BankingAccount bankingAccount)
        {
            try
            {
                this._dbContext.BankingAccounts.Update(bankingAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
