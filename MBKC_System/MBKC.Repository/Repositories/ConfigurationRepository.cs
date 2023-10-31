using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class ConfigurationRepository
    {
        private MBKCDbContext _dbContext;
        public ConfigurationRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<Configuration>> GetConfigurationsAsync()
        {
            try
            {
                return await this._dbContext.Configurations.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void UpdateConfiguration(Configuration configuration)
        {
            try
            {
                this._dbContext.Configurations.Update(configuration);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
