using MBKC.Repository.Redis.Models;
using Redis.OM.Searching;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Redis.Repositories
{
    public class AccountTokenRedisRepository
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<AccountToken> _accounttokenCollection;
        public AccountTokenRedisRepository(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._accounttokenCollection = this._redisConnectionProvider.RedisCollection<AccountToken>();
        }

        public async Task AddAccountToken(AccountToken accountToken)
        {
            try
            {
                await this._accounttokenCollection.InsertAsync(accountToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AccountToken> GetAccountToken(string accountId)
        {
            try
            {
                return await this._accounttokenCollection.SingleOrDefaultAsync(x => x.AccountId == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAccountToken(AccountToken accountToken)
        {
            try
            {
                await this._accounttokenCollection.UpdateAsync(accountToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAccountToken(AccountToken accountToken)
        {
            try
            {
                await this._accounttokenCollection.DeleteAsync(accountToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
