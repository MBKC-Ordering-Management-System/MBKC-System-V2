using MBKC.DAL.DBContext;
using MBKC.DAL.Enums;
using MBKC.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.DAOs
{
    public class AccountDAO
    {
        private MBKCDbContext _dbContext;
        public AccountDAO(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Account> GetAccountAsync(string email, string password)
        {
            try
            {
                return await this._dbContext.Accounts.Include(x => x.Role)
                                                     .SingleOrDefaultAsync(x => x.Email.Equals(email) && x.Password.Equals(password) && x.Status == Convert.ToBoolean((int)AccountEnum.Status.ACTIVE));
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
                                                     .SingleOrDefaultAsync(x => x.AccountId == accountId);
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
                                                     .SingleOrDefaultAsync(x => x.Email == email);
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
