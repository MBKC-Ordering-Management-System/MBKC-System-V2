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
    public class StorePartnerRepository
    {
        private MBKCDbContext _dbContext;
        public StorePartnerRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<StorePartner> GetStorePartnerByPartnerIdAndStoreIdAsync(int partnerId, int storeId)
        {
            try
            {
                return await this._dbContext.StorePartners.SingleOrDefaultAsync(s => s.PartnerId == partnerId && s.StoreId == storeId && s.Status != (int)StorePartnerEnum.Status.DEACTIVE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<StorePartner>> GetStorePartnersByUserNameAndStoreIdAsync(string userName, int storeId, int partnerId)
        {
            try
            {
                return await this._dbContext.StorePartners.Where(s => s.StoreId != storeId && s.UserName.Equals(userName) && s.PartnerId == partnerId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<StorePartner>> GetStorePartnersByStoreIdAndBrandIdAsync(string? searchValue, int? currentPage, int? itemsPerPage, int storeId, int brandId)
        {
            try
            {
                return await this._dbContext.StorePartners.Where(s => s.StoreId == storeId && s.Status != (int)StorePartnerEnum.Status.DEACTIVE).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateStorePartnerAsync(StorePartner storePartner)
        {
            try
            {
                await this._dbContext.AddAsync(storePartner);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateStorePartner(StorePartner storePartner)
        {
            try
            {
                this._dbContext.StorePartners.Update(storePartner);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
