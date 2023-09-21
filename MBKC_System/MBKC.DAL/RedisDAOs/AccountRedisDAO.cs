using Redis.OM.Searching;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.DAL.RedisModels;
using Microsoft.EntityFrameworkCore;
using MBKC.DAL.Enums;
using MBKC.DAL.Models;

namespace MBKC.DAL.RedisDAOs
{
    public class AccountRedisDAO
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<AccountRedisModel> _accountCollection;
        public AccountRedisDAO(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._accountCollection = this._redisConnectionProvider.RedisCollection<AccountRedisModel>();
        }

        public async Task AddAccountAsync(AccountRedisModel account)
        {
            try
            {
                await this._accountCollection.InsertAsync(account);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AccountRedisModel> GetAccountAsync(string accountId)
        {
            try
            {
                return await this._accountCollection.FirstOrDefaultAsync(x => x.AccountId == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AccountRedisModel> GetAccountWithEmailAsync(string email)
        {
            try
            {
                return await this._accountCollection.FirstOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AccountRedisModel> GetAccountAsync(string email, string password)
        {
            try
            {
                bool activeStatus = Convert.ToBoolean((int)AccountEnum.Status.ACTIVE);
                return await this._accountCollection.SingleOrDefaultAsync(x => x.Email == email && x.Password == password && x.Status == activeStatus);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAccountAsync(AccountRedisModel account)
        {
            try
            {
                await this._accountCollection.UpdateAsync(account);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAccountAsync(AccountRedisModel account)
        {
            try
            {
                await this._accountCollection.DeleteAsync(account);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
