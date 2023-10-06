using MBKC.Repository.RedisModels;
using Redis.OM.Searching;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.RedisRepositories
{
    public class AccountConfirmationRedisRepository
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<AccountConfirmation> _accountConfirmationCollection;
        public AccountConfirmationRedisRepository(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._accountConfirmationCollection = this._redisConnectionProvider.RedisCollection<AccountConfirmation>();
        }

        public async Task CreateAccountConfirmationAsync(AccountConfirmation accountConfirmation)
        {
            try
            {
                await this._accountConfirmationCollection.InsertAsync(accountConfirmation);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AccountConfirmation> GetAccountConfirmationAsync(string accountId)
        {
            try
            {
                return await this._accountConfirmationCollection.FirstOrDefaultAsync(x => x.AccountId == accountId);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        
        public async Task UpdateAccountConfirmationAsync(AccountConfirmation accountConfirmation)
        {
            try
            {
                await this._accountConfirmationCollection.UpdateAsync(accountConfirmation);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
