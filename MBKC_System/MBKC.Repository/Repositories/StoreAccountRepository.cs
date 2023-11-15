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
    public class StoreAccountRepository
    {
        private MBKCDbContext _dbContext;
        public StoreAccountRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task AddStoreAccountAsync(StoreAccount storeAccount)
        {
            try
            {
                await this._dbContext.StoreAccounts.AddAsync(storeAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<StoreAccount> GetStoreAccountAsync(int accountId)
        {
            try
            {
                return await this._dbContext.StoreAccounts
                    .Include(x => x.Store)
                    .ThenInclude(x => x.StoreMoneyExchanges)
                    .Include(x => x.Store)
                    .ThenInclude(x => x.Wallet)
                    .ThenInclude(x => x.Transactions)
                    .ThenInclude(x => x.MoneyExchange)
                    .Include(x => x.Store)
                    .ThenInclude(x => x.Wallet)
                    .ThenInclude(x => x.Transactions)
                    .ThenInclude(x => x.ShipperPayment)
                    .ThenInclude(x => x.Order)
                    .Include(x => x.Store)
                    .ThenInclude(x => x.Wallet)
                    .ThenInclude(x => x.Transactions)
                    .ThenInclude(x => x.ShipperPayment)
                    .ThenInclude(x => x.BankingAccount)
                    .Include(x => x.Store).ThenInclude(x => x.KitchenCenter).ThenInclude(x => x.Cashiers)
                    .Include(x => x.Store).ThenInclude(x => x.Orders).ThenInclude(x => x.OrderHistories)
                    .Include(x => x.Store).ThenInclude(x => x.Orders).ThenInclude(x => x.ShipperPayments)
                    .SingleOrDefaultAsync(x => x.AccountId == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<StoreAccount> GetStoreAccountWalletAsync(int accountId)
        {
            try
            {
                return await this._dbContext.StoreAccounts
                    .Include(x => x.Store).ThenInclude(x => x.Wallet)
                    .Include(x => x.Store).ThenInclude(x => x.Orders).ThenInclude(x => x.ShipperPayments)
                    .Include(x => x.Store).ThenInclude(x => x.Orders).ThenInclude(x => x.OrderHistories)
                    .SingleOrDefaultAsync(x => x.AccountId == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
