using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
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
                                                          .Where(x => x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                          (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true))
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
                                                                     x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                                     (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)).CountAsync();


                }
                return await this._dbContext.PartnerProducts.Include(x => x.Product)
                                                         .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                         .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                         .Where(x => x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                         (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)).CountAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetPartnerProductsAsync
        public async Task<List<PartnerProduct>> GetPartnerProductsAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int? brandId)
        {
            try
            {
                if (searchValue == null && searchValueWithoutUnicode is not null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                          .Where(x => x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                          (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true))
                                                         .Where(delegate (PartnerProduct partnerProduct)
                                                         {
                                                             if (searchValueWithoutUnicode.ToLower().Contains(StringUtil.RemoveSign4VietnameseString(partnerProduct.Product.Name).ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         })
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("productname"),
                                                                  then => then.OrderBy(x => x.Product.Name))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("partnername"),
                                                                  then => then.OrderBy(x => x.StorePartner.Partner.Name))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("storename"),
                                                                  then => then.OrderBy(x => x.StorePartner.Store.Name))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("productcode"),
                                                                  then => then.OrderBy(x => x.ProductCode))
                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Status))
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                          .Where(x => x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                          (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true))
                                                         .Where(delegate (PartnerProduct partnerProduct)
                                                         {
                                                             if (searchValueWithoutUnicode.ToLower().Contains(StringUtil.RemoveSign4VietnameseString(partnerProduct.Product.Name).ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         })
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("productname"),
                                                                  then => then.OrderByDescending(x => x.Product.Name))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("partnername"),
                                                                  then => then.OrderByDescending(x => x.StorePartner.Partner.Name))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("storename"),
                                                                  then => then.OrderByDescending(x => x.StorePartner.Store.Name))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("productcode"),
                                                                  then => then.OrderByDescending(x => x.ProductCode))
                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                  then => then.OrderByDescending(x => x.Status))
                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                          .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                          .Where(x => x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                          (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true))
                                                         .Where(delegate (PartnerProduct partnerProduct)
                                                         {
                                                             if (searchValueWithoutUnicode.ToLower().Contains(StringUtil.RemoveSign4VietnameseString(partnerProduct.Product.Name).ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }

                else if (searchValue != null && searchValueWithoutUnicode is null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                                .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                                .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                                .Where(x => x.Product.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                            x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                                     (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true))
                                                                .If(sortByASC != null && sortByASC.ToLower().Equals("productname"),
                                                                  then => then.OrderBy(x => x.Product.Name))
                                                                .If(sortByASC != null && sortByASC.ToLower().Equals("partnername"),
                                                                  then => then.OrderBy(x => x.StorePartner.Partner.Name))
                                                                .If(sortByASC != null && sortByASC.ToLower().Equals("storename"),
                                                                  then => then.OrderBy(x => x.StorePartner.Store.Name))
                                                                .If(sortByASC != null && sortByASC.ToLower().Equals("productcode"),
                                                                  then => then.OrderBy(x => x.ProductCode))
                                                                .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Status))
                                                                .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                                .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                                .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                                .Where(x => x.Product.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                            x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                                     (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true))
                                                                .If(sortByDESC != null && sortByDESC.ToLower().Equals("productname"),
                                                                  then => then.OrderByDescending(x => x.Product.Name))
                                                                .If(sortByDESC != null && sortByDESC.ToLower().Equals("partnername"),
                                                                  then => then.OrderByDescending(x => x.StorePartner.Partner.Name))
                                                                .If(sortByDESC != null && sortByDESC.ToLower().Equals("storename"),
                                                                  then => then.OrderByDescending(x => x.StorePartner.Store.Name))
                                                                .If(sortByDESC != null && sortByDESC.ToLower().Equals("productcode"),
                                                                  then => then.OrderByDescending(x => x.ProductCode))
                                                                .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                  then => then.OrderByDescending(x => x.Status))
                                                                .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                                .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                                .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                                .Where(x => x.Product.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                            x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                                     (brandId != null
                                                                     ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                     : true)).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }

                if (sortByASC is not null)
                    return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                            .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                            .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                            .Where(x => x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                             (brandId != null
                                                                  ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                  : true))
                                                            .If(sortByASC != null && sortByASC.ToLower().Equals("productname"),
                                                                  then => then.OrderBy(x => x.Product.Name))
                                                            .If(sortByASC != null && sortByASC.ToLower().Equals("partnername"),
                                                                  then => then.OrderBy(x => x.StorePartner.Partner.Name))
                                                            .If(sortByASC != null && sortByASC.ToLower().Equals("storename"),
                                                                  then => then.OrderBy(x => x.StorePartner.Store.Name))
                                                            .If(sortByASC != null && sortByASC.ToLower().Equals("productcode"),
                                                                  then => then.OrderBy(x => x.ProductCode))
                                                            .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                  then => then.OrderBy(x => x.Status))
                                                            .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                else if (sortByDESC is not null)
                    return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                            .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                            .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                            .Where(x => x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                             (brandId != null
                                                                  ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                  : true))
                                                            .If(sortByDESC != null && sortByDESC.ToLower().Equals("productname"),
                                                                  then => then.OrderByDescending(x => x.Product.Name))
                                                            .If(sortByDESC != null && sortByDESC.ToLower().Equals("partnername"),
                                                                  then => then.OrderByDescending(x => x.StorePartner.Partner.Name))
                                                            .If(sortByDESC != null && sortByDESC.ToLower().Equals("storename"),
                                                                  then => then.OrderByDescending(x => x.StorePartner.Store.Name))
                                                            .If(sortByDESC != null && sortByDESC.ToLower().Equals("productcode"),
                                                                  then => then.OrderByDescending(x => x.ProductCode))
                                                            .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                  then => then.OrderByDescending(x => x.Status))
                                                            .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return this._dbContext.PartnerProducts.Include(x => x.Product)
                                                            .Include(x => x.StorePartner).ThenInclude(x => x.Store).ThenInclude(x => x.Brand)
                                                            .Include(x => x.StorePartner).ThenInclude(x => x.Partner)
                                                            .Where(x => x.Status != (int)PartnerProductEnum.Status.DEACTIVE &&
                                                             (brandId != null
                                                                  ? x.StorePartner.Store.Brand.BrandId == brandId
                                                                  : true)).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
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
