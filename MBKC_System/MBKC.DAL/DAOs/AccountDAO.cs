using MBKC.DAL.DBContext;
using MBKC.DAL.Models;
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

        public async Task CreateAccount(Account account)
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
    }
}
