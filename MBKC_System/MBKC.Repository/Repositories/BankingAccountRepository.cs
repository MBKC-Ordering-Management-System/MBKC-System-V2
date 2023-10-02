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
                                                                         && x.Status != (int)BankingAccountEnum.Status.DEACTIVE).CountAsync();
                } else if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.BankingAccounts.Where(x => x.KitchenCenterId == kitchenCenterId && x.Status != (int)BankingAccountEnum.Status.DEACTIVE).Where(delegate (BankingAccount bankingAccount)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(bankingAccount.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        return false;
                    }).AsQueryable().Count();
                }
                return await this._dbContext.BankingAccounts.Where(x => x.KitchenCenterId == kitchenCenterId && x.Status != (int)BankingAccountEnum.Status.DEACTIVE).CountAsync();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BankingAccount>> GetBankingAccountsAsync(int kitchenCenterId, string? searchValue, string? searchValueWithoutUnicode, int currentPage, int itemsPerPage)
        {
            try
            {
                if(searchValue != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.BankingAccounts.Where(x => x.KitchenCenterId == kitchenCenterId 
                                                                         && x.Status != (int)BankingAccountEnum.Status.DEACTIVE
                                                                         && x.Name.ToLower().Contains(searchValue.ToLower())).ToListAsync();
                } else if(searchValue == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.BankingAccounts.Where(x => x.KitchenCenterId == kitchenCenterId
                                                                         && x.Status != (int)BankingAccountEnum.Status.DEACTIVE)
                                                                .Where(delegate (BankingAccount bankingAccount)
                                                                {
                                                                    if (StringUtil.RemoveSign4VietnameseString(bankingAccount.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                                    {
                                                                        return true;
                                                                    }
                                                                    return false;
                                                                }).AsQueryable().ToList();
                }
                return await this._dbContext.BankingAccounts.Where(x => x.KitchenCenterId == kitchenCenterId && x.Status != (int)BankingAccountEnum.Status.DEACTIVE).ToListAsync();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BankingAccount> GetBankingAccountAsync(int bankingAccountId)
        {
            try
            {
                return await this._dbContext.BankingAccounts.FirstOrDefaultAsync(x => x.BankingAccountId == bankingAccountId && x.Status != (int)BankingAccountEnum.Status.DEACTIVE);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BankingAccount> GetBankingAccountAsync(string numberAccount)
        {
            try
            {
                return await this._dbContext.BankingAccounts.FirstOrDefaultAsync(x => x.NumberAccount.Equals(numberAccount));
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateBankingAccountAsync(BankingAccount bankingAccount)
        {
            try
            {
                await this._dbContext.BankingAccounts.AddAsync(bankingAccount);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateBankingAccount(BankingAccount bankingAccount)
        {
            try
            {
                this._dbContext.BankingAccounts.Update(bankingAccount);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
