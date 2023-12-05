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

        public async Task<Product> GetProductAsync(string code, int brandId)
        {
            try
            {
                return await this._dbContext.Products.SingleOrDefaultAsync(x => x.Code.ToLower().Equals(code.ToLower()) && x.Brand.BrandId == brandId && x.Status != (int)ProductEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Product> CheckProductCodeInBrandAsync(string code, int brandId)
        {
            try
            {
                return await this._dbContext.Products.SingleOrDefaultAsync(x => x.Code.ToLower().Equals(code.ToLower()) && x.Brand.BrandId == brandId && x.Status != (int)ProductEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Product> CheckProductNameInBrandAsync(string productName, int brandId)
        {
            try
            {
                return await this._dbContext.Products.SingleOrDefaultAsync(x => x.Name.ToLower().Equals(productName.ToLower()) && x.Brand.BrandId == brandId && x.Status != (int)ProductEnum.Status.DISABLE);
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

        public async Task CreateRangProductAsync(IEnumerable<Product> products)
        {
            try
            {
                await this._dbContext.Products.AddRangeAsync(products);
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
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
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
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
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
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
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

        public async Task<int> GetNumberProductWithNumberProductsSoldAsync(string? searchName, string? searchValueWithoutUnicode, string? productType,
           int? idCategory, int? brandId)
        {
            try
            {
                if (searchName == null && searchValueWithoutUnicode != null)
                {
                    return this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) || 
                                                                        x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
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
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                     x.Name.ToLower().Contains(searchName.ToLower()) &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                        x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true)).CountAsync();
                }
                return await this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                        x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true)).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Product>> GetProductsAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, string? productType, int? idCategory, int? storeId, int? brandId, int? kitchenCenterId, bool? isGetAll)
        {
            try
            {
                if (isGetAll != null && isGetAll == true)
                {
                    return await this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Include(x => x.ParentProduct)
                                                         .Include(x => x.ChildrenProducts)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
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
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
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
                                                             }).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                                                  then => then.OrderBy(x => x.Code))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                 then => then.OrderBy(x => x.Name))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("sellingprice"),
                                                                         then => then.OrderBy(x => x.SellingPrice))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("discountprice"),
                                                                         then => then.OrderBy(x => x.DiscountPrice))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                                                         then => then.OrderBy(x => x.DisplayOrder))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("historicalprice"),
                                                                         then => then.OrderBy(x => x.HistoricalPrice))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                         then => then.OrderBy(x => x.Status).Reverse())
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
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
                                                             }).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                                                                        then => then.OrderByDescending(x => x.Code))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                         then => then.OrderByDescending(x => x.Name))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("sellingprice"),
                                                                         then => then.OrderByDescending(x => x.SellingPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("discountprice"),
                                                                         then => then.OrderByDescending(x => x.DiscountPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                                                                         then => then.OrderByDescending(x => x.DisplayOrder))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("historicalprice"),
                                                                         then => then.OrderByDescending(x => x.HistoricalPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                         then => then.OrderByDescending(x => x.Status).Reverse())
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Products.Include(x => x.Category)
                                                   .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                   .Include(x => x.ParentProduct)
                                                   .Include(x => x.ChildrenProducts)
                                                   .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
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
                                                   }).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                         x.Name.ToLower().Contains(searchValue.ToLower()) &&
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
                                                                         : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                                                                   then => then.OrderBy(x => x.Code))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                                   then => then.OrderBy(x => x.Name))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("sellingprice"),
                                                                                   then => then.OrderBy(x => x.SellingPrice))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("discountprice"),
                                                                                   then => then.OrderBy(x => x.DiscountPrice))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                                                                   then => then.OrderBy(x => x.DisplayOrder))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("historicalprice"),
                                                                                   then => then.OrderBy(x => x.HistoricalPrice))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                                   then => then.OrderBy(x => x.Status).Reverse())
                                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Include(x => x.ParentProduct)
                                                         .Include(x => x.ChildrenProducts)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                     x.Name.ToLower().Contains(searchValue.ToLower()) &&
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
                                                                     : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                                                                               then => then.OrderByDescending(x => x.Code))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                               then => then.OrderByDescending(x => x.Name))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("sellingprice"),
                                                                               then => then.OrderByDescending(x => x.SellingPrice))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("discountprice"),
                                                                               then => then.OrderByDescending(x => x.DiscountPrice))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                                                                               then => then.OrderByDescending(x => x.DisplayOrder))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("historicalprice"),
                                                                               then => then.OrderByDescending(x => x.HistoricalPrice))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                               then => then.OrderByDescending(x => x.Status).Reverse())
                                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Include(x => x.ParentProduct)
                                                         .Include(x => x.ChildrenProducts)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                     x.Name.ToLower().Contains(searchValue.ToLower()) &&
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
                                                                     : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                }

                if (sortByASC is not null)
                    return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
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
                                                                         : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                                                                   then => then.OrderBy(x => x.Code))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                                    then => then.OrderBy(x => x.Name))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("sellingprice"),
                                                                                    then => then.OrderBy(x => x.SellingPrice))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("discountprice"),
                                                                                    then => then.OrderBy(x => x.DiscountPrice))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                                                                    then => then.OrderBy(x => x.DisplayOrder))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("historicalprice"),
                                                                                    then => then.OrderBy(x => x.HistoricalPrice))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                                    then => then.OrderBy(x => x.Status).Reverse())
                                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                else if (sortByDESC is not null)
                    return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
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
                                                                         : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                                                                                    then => then.OrderByDescending(x => x.Code))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                                    then => then.OrderByDescending(x => x.Name))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("sellingprice"),
                                                                                    then => then.OrderByDescending(x => x.SellingPrice))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("discountprice"),
                                                                                    then => then.OrderByDescending(x => x.DiscountPrice))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                                                                                    then => then.OrderByDescending(x => x.DisplayOrder))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("historicalprice"),
                                                                                    then => then.OrderByDescending(x => x.HistoricalPrice))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                                    then => then.OrderByDescending(x => x.Status).Reverse())
                                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
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
                                                                         : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Product>> GetProductsWithNumberProductsSoldAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, string? productType, int? idCategory, int? brandId, bool? isGetAll)
        {
            try
            {
                if (isGetAll != null && isGetAll == true)
                {
                    return await this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Include(x => x.ParentProduct)
                                                         .Include(x => x.ChildrenProducts)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                        x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync();
                }
                if (searchValue == null && searchValueWithoutUnicode != null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                         (productType != null
                                                                         ? x.Type.ToLower().Equals(productType.ToLower())
                                                                         : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                            x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                         (idCategory != null
                                                                         ? x.Category.CategoryId == idCategory
                                                                         : true) &&
                                                                         (brandId != null
                                                                         ? x.Brand.BrandId == brandId
                                                                         : true))
                                                             .Where(delegate (Product product)
                                                             {
                                                                 if (StringUtil.RemoveSign4VietnameseString(product.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                                 {
                                                                     return true;
                                                                 }
                                                                 return false;
                                                             }).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                                                  then => then.OrderBy(x => x.Code))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                 then => then.OrderBy(x => x.Name))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("sellingprice"),
                                                                         then => then.OrderBy(x => x.SellingPrice))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("discountprice"),
                                                                         then => then.OrderBy(x => x.DiscountPrice))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                                                         then => then.OrderBy(x => x.DisplayOrder))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("historicalprice"),
                                                                         then => then.OrderBy(x => x.HistoricalPrice))
                                                              .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                         then => then.OrderBy(x => x.Status).Reverse())
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                         (productType != null
                                                                         ? x.Type.ToLower().Equals(productType.ToLower())
                                                                         : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                            x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                         (idCategory != null
                                                                         ? x.Category.CategoryId == idCategory
                                                                         : true) &&
                                                                         (brandId != null
                                                                         ? x.Brand.BrandId == brandId
                                                                         : true))
                                                             .Where(delegate (Product product)
                                                             {
                                                                 if (StringUtil.RemoveSign4VietnameseString(product.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                                 {
                                                                     return true;
                                                                 }
                                                                 return false;
                                                             }).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                                                                        then => then.OrderByDescending(x => x.Code))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                         then => then.OrderByDescending(x => x.Name))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("sellingprice"),
                                                                         then => then.OrderByDescending(x => x.SellingPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("discountprice"),
                                                                         then => then.OrderByDescending(x => x.DiscountPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                                                                         then => then.OrderByDescending(x => x.DisplayOrder))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("historicalprice"),
                                                                         then => then.OrderByDescending(x => x.HistoricalPrice))
                                                              .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                         then => then.OrderByDescending(x => x.Status).Reverse())
                                                              .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();

                    return this._dbContext.Products.Include(x => x.Category)
                                                   .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                   .Include(x => x.ParentProduct)
                                                   .Include(x => x.ChildrenProducts)
                                                   .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                               (productType != null
                                                               ? x.Type.ToLower().Equals(productType.ToLower())
                                                               : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                  x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                               (idCategory != null
                                                               ? x.Category.CategoryId == idCategory
                                                               : true) &&
                                                               (brandId != null
                                                               ? x.Brand.BrandId == brandId
                                                               : true))
                                                   .Where(delegate (Product product)
                                                   {
                                                       if (StringUtil.RemoveSign4VietnameseString(product.Name).ToLower().Contains(searchValueWithoutUnicode.ToLower()))
                                                       {
                                                           return true;
                                                       }
                                                       return false;
                                                   }).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                     .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).AsQueryable().ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                         x.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                         (productType != null
                                                                         ? x.Type.ToLower().Equals(productType.ToLower())
                                                                         : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                            x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                         (idCategory != null
                                                                         ? x.Category.CategoryId == idCategory
                                                                         : true) &&
                                                                         (brandId != null
                                                                         ? x.Brand.BrandId == brandId
                                                                         : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                                                                   then => then.OrderBy(x => x.Code))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                                   then => then.OrderBy(x => x.Name))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("sellingprice"),
                                                                                   then => then.OrderBy(x => x.SellingPrice))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("discountprice"),
                                                                                   then => then.OrderBy(x => x.DiscountPrice))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                                                                   then => then.OrderBy(x => x.DisplayOrder))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("historicalprice"),
                                                                                   then => then.OrderBy(x => x.HistoricalPrice))
                                                                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                                   then => then.OrderBy(x => x.Status).Reverse())
                                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Include(x => x.ParentProduct)
                                                         .Include(x => x.ChildrenProducts)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                     x.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                        x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                                                                               then => then.OrderByDescending(x => x.Code))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                               then => then.OrderByDescending(x => x.Name))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("sellingprice"),
                                                                               then => then.OrderByDescending(x => x.SellingPrice))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("discountprice"),
                                                                               then => then.OrderByDescending(x => x.DiscountPrice))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                                                                               then => then.OrderByDescending(x => x.DisplayOrder))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("historicalprice"),
                                                                               then => then.OrderByDescending(x => x.HistoricalPrice))
                                                                    .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                               then => then.OrderByDescending(x => x.Status).Reverse())
                                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                    return this._dbContext.Products.Include(x => x.Category)
                                                         .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                         .Include(x => x.ParentProduct)
                                                         .Include(x => x.ChildrenProducts)
                                                         .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                     x.Name.ToLower().Contains(searchValue.ToLower()) &&
                                                                     (productType != null
                                                                     ? x.Type.ToLower().Equals(productType.ToLower())
                                                                     : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                        x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                     (idCategory != null
                                                                     ? x.Category.CategoryId == idCategory
                                                                     : true) &&
                                                                     (brandId != null
                                                                     ? x.Brand.BrandId == brandId
                                                                     : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                }

                if (sortByASC is not null)
                    return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                         (productType != null
                                                                         ? x.Type.ToLower().Equals(productType.ToLower())
                                                                         : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                            x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                         (idCategory != null
                                                                         ? x.Category.CategoryId == idCategory
                                                                         : true) &&
                                                                         (brandId != null
                                                                         ? x.Brand.BrandId == brandId
                                                                         : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                                                                   then => then.OrderBy(x => x.Code))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                                                                    then => then.OrderBy(x => x.Name))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("sellingprice"),
                                                                                    then => then.OrderBy(x => x.SellingPrice))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("discountprice"),
                                                                                    then => then.OrderBy(x => x.DiscountPrice))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                                                                    then => then.OrderBy(x => x.DisplayOrder))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("historicalprice"),
                                                                                    then => then.OrderBy(x => x.HistoricalPrice))
                                                                         .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                                                                    then => then.OrderBy(x => x.Status).Reverse())
                                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                else if (sortByDESC is not null)
                    return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                         (productType != null
                                                                         ? x.Type.ToLower().Equals(productType.ToLower())
                                                                         : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                            x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                         (idCategory != null
                                                                         ? x.Category.CategoryId == idCategory
                                                                         : true) &&
                                                                         (brandId != null
                                                                         ? x.Brand.BrandId == brandId
                                                                         : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                                                                                    then => then.OrderByDescending(x => x.Code))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                                                                    then => then.OrderByDescending(x => x.Name))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("sellingprice"),
                                                                                    then => then.OrderByDescending(x => x.SellingPrice))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("discountprice"),
                                                                                    then => then.OrderByDescending(x => x.DiscountPrice))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                                                                                    then => then.OrderByDescending(x => x.DisplayOrder))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("historicalprice"),
                                                                                    then => then.OrderByDescending(x => x.HistoricalPrice))
                                                                         .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                                                                    then => then.OrderByDescending(x => x.Status).Reverse())
                                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return this._dbContext.Products.Include(x => x.Category)
                                                             .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                                             .Include(x => x.ParentProduct)
                                                             .Include(x => x.ChildrenProducts)
                                                             .Where(x => x.Status != (int)ProductEnum.Status.DISABLE &&
                                                                         (productType != null
                                                                         ? x.Type.ToLower().Equals(productType.ToLower())
                                                                         : (x.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()) ||
                                                                            x.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))) &&
                                                                         (idCategory != null
                                                                         ? x.Category.CategoryId == idCategory
                                                                         : true) &&
                                                                         (brandId != null
                                                                         ? x.Brand.BrandId == brandId
                                                                         : true)).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                                                                         .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();


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
                                        .Include(x => x.PartnerProducts).ThenInclude(x => x.StorePartner).ThenInclude(x => x.Partner)
                                        .Include(x => x.PartnerProducts).ThenInclude(x => x.StorePartner).ThenInclude(x => x.Store)
                                        .Include(x => x.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                                        .Include(x => x.ParentProduct).ThenInclude(x => x.PartnerProducts)
                                        .Include(x => x.ChildrenProducts)
                                        .SingleOrDefaultAsync(x => x.ProductId == id && x.Status != (int)ProductEnum.Status.DISABLE);
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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Count product number id brand id
        public async Task<int> CountProductNumberByBrandIdAsync(int brandId)
        {
            try
            {
                return await this._dbContext.Products.Where(p => (p.Status == (int)ProductEnum.Status.ACTIVE || p.Status == (int)ProductEnum.Status.INACTIVE)
                                                               && p.Brand.BrandId == brandId)
                                                     .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
