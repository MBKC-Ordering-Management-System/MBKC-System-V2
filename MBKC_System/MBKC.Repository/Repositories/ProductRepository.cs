using MBKC.Repository.DBContext;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Repository.Utils;
using Microsoft.EntityFrameworkCore;

namespace MBKC.Repository.Repositories
{
    public class ProductRepository
    {
        private MBKCDbContext _dbContext;
        public ProductRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Product> GetProductAsync(string code)
        {
            try
            {
                return await this._dbContext.Products.SingleOrDefaultAsync(x => x.Code.ToLower().Equals(code.ToLower()));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateProductAsync(Product product)
        {
            try
            {
                await this._dbContext.Products.AddAsync(product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<int> GetNumberProductsAsync(string? searchName, string? searchValueWithoutUnicode, string? productType,
            int? idCategory, int? storeId, int? brandId, int? kitchenCenterId)
        {
            try
            {
                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DEACTIVE &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : true) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (storeId != null
                                                                     ? x.Brand.Stores.Any(store => store.StoreId == storeId)
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Brand.Stores.Any(store => store.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                                     : true))
                                                         .Where(delegate (Product product)
                                                         {
                                                             if (StringUtil.RemoveSign4VietnameseString(product.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).AsQueryable().Count();
                }
                else if (searchName != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DEACTIVE &&
                                                                     x.Name.ToLower().Contains(searchName.ToLower()) &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : true) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (storeId != null
                                                                     ? x.Brand.Stores.Any(store => store.StoreId == storeId)
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Brand.Stores.Any(store => store.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                                     : true)).CountAsync();
                }
                return await this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DEACTIVE &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : true) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (storeId != null
                                                                     ? x.Brand.Stores.Any(store => store.StoreId == storeId)
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Brand.Stores.Any(store => store.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                                     : true)).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Product>> GetProductsAsync(string? searchName, string? searchValueWithoutUnicode, string? productType,
            int? idCategory, int? storeId, int? brandId, int? kitchenCenterId, int? currentPage, int? itemsPerPage)
        {
            try
            {
                if (currentPage != null && itemsPerPage != null)
                {
                    if (searchName == null && searchValueWithoutUnicode != null)
                    {
                        return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DEACTIVE &&
                                                                         (productType != null
                                                                         ? x.Type.ToLower().Equals(productType.ToLower())
                                                                         : true) &&
                                                                         (idCategory != null
                                                                         ? x.Category.CategoryId == idCategory
                                                                         : true) &&
                                                                         (storeId != null
                                                                         ? x.Brand.Stores.Any(store => store.StoreId == storeId)
                                                                         : true) &&
                                                                         (brandId != null
                                                                         ? x.Brand.BrandId == brandId
                                                                         : true) &&
                                                                         (kitchenCenterId != null
                                                                         ? x.Brand.Stores.Any(store => store.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                                         : true))
                                                             .Where(delegate (Product product)
                                                             {
                                                                 if (StringUtil.RemoveSign4VietnameseString(product.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                                 {
                                                                     return true;
                                                                 }
                                                                 return false;
                                                             }).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).AsQueryable().ToList();
                    }
                    else if (searchName != null && searchValueWithoutUnicode == null)
                    {
                        return await this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DEACTIVE &&
                                                                         x.Name.ToLower().Contains(searchName.ToLower()) &&
                                                                         (productType != null
                                                                         ? x.Type.ToLower().Equals(productType.ToLower())
                                                                         : true) &&
                                                                         (idCategory != null
                                                                         ? x.Category.CategoryId == idCategory
                                                                         : true) &&
                                                                         (storeId != null
                                                                         ? x.Brand.Stores.Any(store => store.StoreId == storeId)
                                                                         : true) &&
                                                                         (brandId != null
                                                                         ? x.Brand.BrandId == brandId
                                                                         : true) &&
                                                                         (kitchenCenterId != null
                                                                         ? x.Brand.Stores.Any(store => store.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                                         : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
                    }
                    return await this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DEACTIVE &&
                                                                         (productType != null
                                                                         ? x.Type.ToLower().Equals(productType.ToLower())
                                                                         : true) &&
                                                                         (idCategory != null
                                                                         ? x.Category.CategoryId == idCategory
                                                                         : true) &&
                                                                         (storeId != null
                                                                         ? x.Brand.Stores.Any(store => store.StoreId == storeId)
                                                                         : true) &&
                                                                         (brandId != null
                                                                         ? x.Brand.BrandId == brandId
                                                                         : true) &&
                                                                         (kitchenCenterId != null
                                                                         ? x.Brand.Stores.Any(store => store.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                                         : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
                }
                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Include(x => x.ParentProduct)
                                                         .Include(x => x.ChildrenProducts)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DEACTIVE &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : true) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (storeId != null
                                                                     ? x.Brand.Stores.Any(store => store.StoreId == storeId)
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Brand.Stores.Any(store => store.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                                     : true))
                                                         .Where(delegate (Product product)
                                                         {
                                                             if (StringUtil.RemoveSign4VietnameseString(product.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                             {
                                                                 return true;
                                                             }
                                                             return false;
                                                         }).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).AsQueryable().ToList();
                }
                else if (searchName != null && searchValueWithoutUnicode == null)
                {
                    return await this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DEACTIVE &&
                                                                     x.Name.ToLower().Contains(searchName.ToLower()) &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : true) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (storeId != null
                                                                     ? x.Brand.Stores.Any(store => store.StoreId == storeId)
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Brand.Stores.Any(store => store.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                                     : true)).ToListAsync();
                }
                return await this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Include(x => x.ParentProduct)
                                                         .Include(x => x.ChildrenProducts)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DEACTIVE &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : true) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (storeId != null
                                                                     ? x.Brand.Stores.Any(store => store.StoreId == storeId)
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true) &&
                                                                     (kitchenCenterId != null
                                                                     ? x.Brand.Stores.Any(store => store.KitchenCenter.KitchenCenterId == kitchenCenterId)
                                                                     : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Product> GetProductAsync(int id)
        {
            try
            {
                return await this._dbContext.Products.Include(x => x.Category)
                                        .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                        .Include(x => x.ParentProduct)
                                        .Include(x => x.ChildrenProducts)
                                        .SingleOrDefaultAsync(x => x.ProductId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateProduct(Product product)
        {
            try
            {
                this._dbContext.Products.Update(product);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
