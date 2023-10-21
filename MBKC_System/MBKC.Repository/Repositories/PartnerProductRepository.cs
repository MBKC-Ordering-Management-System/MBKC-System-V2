using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.Repositories
{
    public class PartnerProductRepository
    {
        private MBKCDbContext _dbContext;
        public PartnerProductRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region GetPartnerProductAsync
        public async Task<PartnerProduct> GetPartnerProductAsync(int productId, int partnerId, int storeId, DateTime createdDate)
        {
            try
            {
                return await this._dbContext.PartnerProducts
                    .Include(x => x.Product)
                    .Include(x => x.StorePartner)
                    .ThenInclude(x => x.Store)
                    .Include(x => x.StorePartner)
                    .ThenInclude(x => x.Partner)
                    .SingleOrDefaultAsync(mp => mp.ProductId == productId &&
                                          mp.PartnerId == partnerId &&
                                          mp.CreatedDate == createdDate &&
                                          mp.StoreId == storeId);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetPartnerProductByProductCodeAsync
        public async Task<PartnerProduct> GetPartnerProductByProductCodeAsync(string productCode)
        {
            try
            {
                return await this._dbContext.PartnerProducts.SingleOrDefaultAsync(mp => mp.ProductCode.Equals(productCode));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Create Partner Product
        public async Task CreatePartnerProductAsync(PartnerProduct partnerProduct)
        {
            try
            {
                await this._dbContext.PartnerProducts.AddAsync(partnerProduct);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        
        #region Create Range Partner Product
        public async Task CreateRangePartnerProductsAsync(List<PartnerProduct> partnerProducts)
        {
            try
            {
                await this._dbContext.PartnerProducts.AddRangeAsync(partnerProducts);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public void UpdateRangePartnerProductsAsync(List<PartnerProduct> partnerProducts)
        {
            try
            {
                this._dbContext.PartnerProducts.UpdateRange(partnerProducts);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetNumberPartnerProductsAsync
        public async Task<int> GetNumberPartnerProductsAsync(string? searchName, string? searchValueWithoutUnicode, int? brandId)
        {
            try
            {
                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                          .Where(x => brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)
                                                         .Where(delegate (PartnerProduct partnerProduct)
                                                         {
                                                             if (searchValueWithoutUnicode.ToLower().Contains(StringUtil.RemoveSign4VietnameseString(partnerProduct.Product.Name).ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).AsQueryable().Count();
                }
                else if (searchName != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.PartnerProducts.Include(x => x.Product)
                                                         .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                         .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                         .Where(x => x.Product.Name.ToLower().Contains(searchName.ToLower()) &&
                                                                     (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)).CountAsync();


                }
                return await this._dbContext.PartnerProducts.Include(x => x.Product)
                                                         .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                         .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                         .Where(x => brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true).CountAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetPartnerProductsAsync
        public async Task<List<PartnerProduct>> GetPartnerProductsAsync(string? searchName, string? searchValueWithoutUnicode, int? currentPage, int? itemsPerPage, int? brandId)
        {
            try
            {
                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                          .Where(x => brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)
                                                         .Where(delegate (PartnerProduct partnerProduct)
                                                         {
                                                             if (searchValueWithoutUnicode.ToLower().Contains(StringUtil.RemoveSign4VietnameseString(partnerProduct.Product.Name).ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).AsQueryable().ToList();
                }
                else if (searchName != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.PartnerProducts.Include(x => x.Product)
                                                                .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                                .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                                .Where(x => x.Product.Name.ToLower().Contains(searchName.ToLower()) &&
                                                                     (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
                }
                return await this._dbContext.PartnerProducts.Include(x => x.Product)
                                                            .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                            .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                            .Where(x => brandId != null
                                                                  ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                  : true).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region UpdatePartProduct
        public void UpdatePartnerProduct(PartnerProduct partnerProduct)
        {
            try
            {
                this._dbContext.PartnerProducts.Update(partnerProduct);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
