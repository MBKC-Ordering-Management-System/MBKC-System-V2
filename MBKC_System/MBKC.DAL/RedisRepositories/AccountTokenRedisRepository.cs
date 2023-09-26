using MBKC.DAL.RedisModels;
using Redis.OM.Searching;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.RedisRepositories
{
    public class AccountTokenRedisRepository
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<AccountTokenRedisModel> _accounttokenCollection;
        public AccountTokenRedisRepository(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._accounttokenCollection = this._redisConnectionProvider.RedisCollection<AccountTokenRedisModel>();
        }

        public async Task AddAccountToken(AccountTokenRedisModel accountToken)
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

        public async Task<AccountTokenRedisModel> GetAccountToken(string accountId)
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

        public async Task UpdateAccountToken(AccountTokenRedisModel accountToken)
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

        public async Task DeleteAccountToken(AccountTokenRedisModel accountToken)
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
