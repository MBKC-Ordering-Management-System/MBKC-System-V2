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
    public class EmailVerificationRedisRepository
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<EmailVerificationRedisModel> _emailVerificationCollection;
        public EmailVerificationRedisRepository(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._emailVerificationCollection = this._redisConnectionProvider.RedisCollection<EmailVerificationRedisModel>();
        }

        public async Task AddEmailVerificationAsync(EmailVerificationRedisModel emailVerification)
        {
            try { 
                await this._emailVerificationCollection.InsertAsync(emailVerification);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EmailVerificationRedisModel> GetEmailVerificationAsync(string email)
        {
            try
            {
                return await this._emailVerificationCollection.FindByIdAsync(email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateEmailVerificationAsync(EmailVerificationRedisModel emailVerification)
        {
            try
            {
                await this._emailVerificationCollection.UpdateAsync(emailVerification);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteEmailVerificationAsync(EmailVerificationRedisModel emailVerification)
        {
            try
            {
                await this._emailVerificationCollection.DeleteAsync(emailVerification);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
