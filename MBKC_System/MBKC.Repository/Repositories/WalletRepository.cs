using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class WalletRepository
    {
        private MBKCDbContext _dbContext;
        public WalletRepository(MBKCDbContext dbContext)

        {
            this._dbContext = dbContext;
        }

        public void UpdateWallet(Wallet wallet)
        {
            try
            {
                this._dbContext.Wallets.Update(wallet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateWallet(Wallet wallet)
        {
            try
            {
                await this._dbContext.Wallets.AddAsync(wallet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateRangeWallet(IEnumerable<Wallet> wallets)
        {
            try
            {
                this._dbContext.Wallets.UpdateRange(wallets);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}


