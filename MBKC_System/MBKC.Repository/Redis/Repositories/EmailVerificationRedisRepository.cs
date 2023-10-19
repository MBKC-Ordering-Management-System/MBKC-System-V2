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
    public class EmailVerificationRedisRepository
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<EmailVerification> _emailVerificationCollection;
        public EmailVerificationRedisRepository(RedisConnectionProvider redisConnectionProvider)
        {
            this._redisConnectionProvider = redisConnectionProvider;
            this._emailVerificationCollection = this._redisConnectionProvider.RedisCollection<EmailVerification>();
        }

        public async Task AddEmailVerificationAsync(EmailVerification emailVerification)
        {
            try { 
                await this._emailVerificationCollection.InsertAsync(emailVerification);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EmailVerification> GetEmailVerificationAsync(string email)
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

        public async Task UpdateEmailVerificationAsync(EmailVerification emailVerification)
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

        public async Task DeleteEmailVerificationAsync(EmailVerification emailVerification)
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
