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
    public class MappingProductRepository
    {
        private MBKCDbContext _dbContext;
        public MappingProductRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region GetMappingProductAsync
        public async Task<MappingProduct> GetMappingProductAsync(int productId, int partnerId, int storeId, DateTime createdDate)
        {
            try
            {
                return await this._dbContext.MappingProducts
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

        #region GetMappingProductByProductCodeAsync
        public async Task<MappingProduct> GetMappingProductByProductCodeAsync(string productCode)
        {
            try
            {
                return await this._dbContext.MappingProducts.SingleOrDefaultAsync(mp => mp.ProductCode.Equals(productCode));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Create Mapping Product
        public async Task CreateMappingProductAsync(MappingProduct mappingProduct)
        {
            try
            {
                await this._dbContext.MappingProducts.AddAsync(mappingProduct);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetNumberMappingProductsAsync
        public async Task<int> GetNumberMappingProductsAsync(string? searchName, string? searchValueWithoutUnicode, int? brandId)
        {
            try
            {
                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.MappingProducts.Include(x => x.Product)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                          .Where(x => brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)
                                                         .Where(delegate (MappingProduct mappingProduct)
                                                         {
                                                             if (searchValueWithoutUnicode.ToLower().Contains(StringUtil.RemoveSign4VietnameseString(mappingProduct.Product.Name).ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).AsQueryable().Count();
                }
                else if (searchName != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.MappingProducts.Include(x => x.Product)
                                                         .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                         .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                         .Where(x => x.Product.Name.ToLower().Contains(searchName.ToLower()) &&
                                                                     (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)).CountAsync();


                }
                return await this._dbContext.MappingProducts.Include(x => x.Product)
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

        #region GetMappingProductsAsync
        public async Task<List<MappingProduct>> GetMappingProductsAsync(string? searchName, string? searchValueWithoutUnicode, int? currentPage, int? itemsPerPage, int? brandId)
        {
            try
            {
                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.MappingProducts.Include(x => x.Product)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                          .Where(x => brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)
                                                         .Where(delegate (MappingProduct mappingProduct)
                                                         {
                                                             if (searchValueWithoutUnicode.ToLower().Contains(StringUtil.RemoveSign4VietnameseString(mappingProduct.Product.Name).ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).AsQueryable().ToList();
                }
                else if (searchName != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.MappingProducts.Include(x => x.Product)
                                                                .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                                .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                                .Where(x => x.Product.Name.ToLower().Contains(searchName.ToLower()) &&
                                                                     (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
                }
                return await this._dbContext.MappingProducts.Include(x => x.Product)
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

        #region UpdateMappingProduct
        public void UpdateMappingProduct(MappingProduct mappingProduct)
        {
            try
            {
                this._dbContext.MappingProducts.Update(mappingProduct);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
