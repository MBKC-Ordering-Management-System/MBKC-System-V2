using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class AccountRepository
    {
        private MBKCDbContext _dbContext;
        public AccountRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateAccountAsync(Account account)
        {
            try
            {
                await this._dbContext.Accounts.AddAsync(account);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            try
            {
                return await _dbContext.Accounts
                    .Include(a => a.Role)
                    .SingleOrDefaultAsync(r => r.Email.Equals(email) && r.Status != (int)AccountEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Account> GetActiveAccountAsync(string email)
        {
            try
            {
                return await this._dbContext.Accounts.Include(x => x.Role)
                                                     .SingleOrDefaultAsync(x => x.Email.Equals(email) && x.Status == (int)AccountEnum.Status.ACTIVE);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Account> GetAccountAsync(int accountId)
        {
            try
            {
                return await this._dbContext.Accounts.Include(x => x.Role)
                                                     .SingleOrDefaultAsync(x => x.AccountId == accountId && x.Status != (int)AccountEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Account> GetAccountAsync(string email)
        {
            try
            {
                return await this._dbContext.Accounts.Include(x => x.Role)
                                                     .SingleOrDefaultAsync(x => x.Email == email && x.Status != (int)AccountEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateAccount(Account account)
        {
            try
            {
                this._dbContext.Accounts.Update(account);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
