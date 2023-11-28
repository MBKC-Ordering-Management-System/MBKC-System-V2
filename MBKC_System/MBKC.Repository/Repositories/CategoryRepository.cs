using MBKC.Repository.DBContext;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using MBKC.Repository.Enums;
using MBKC.Repository.Utils;
using System.Linq;

namespace MBKC.Repository.Repositories
{
    public class CategoryRepository
    {
        private MBKCDbContext _dbContext;
        public CategoryRepository(MBKCDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #region Get Category By Code
        public async Task<Category> GetCategoryByCodeAsync(string code, int brandId)
        {
            try
            {
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Code.Equals(code) && c.Brand.BrandId == brandId && c.Status != (int)CategoryEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Category By Name
        public async Task<Category> GetCategoryByNameAsync(string categoryName, int brandId)
        {
            try
            {
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Name.ToLower().Equals(categoryName.ToLower()) && c.Brand.BrandId == brandId && c.Status != (int)CategoryEnum.Status.DISABLE);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Category By Code
        public async Task<Category> GetCategoryByDisplayOrderAsync(int displayOrder)
        {
            try
            {
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.DisplayOrder == displayOrder);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Category By Name
        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            try
            {
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Name.Equals(name) && !(c.Status == (int)CategoryEnum.Status.DISABLE));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Category By Id
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _dbContext.Categories
                    .Include(c => c.Brand).ThenInclude(x => x.Stores).ThenInclude(x => x.KitchenCenter)
                    .Include(c => c.ExtraCategoryProductCategories).ThenInclude(x => x.ExtraCategoryNavigation).ThenInclude(x => x.Products)
                    .Include(x => x.ExtraCategoryExtraCategoryNavigations).ThenInclude(x => x.ProductCategory).ThenInclude(x => x.Products)
                    .Include(c => c.Products)
                    .SingleOrDefaultAsync(c => c.CategoryId.Equals(id) && !(c.Status == (int)CategoryEnum.Status.DISABLE));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get only cagtegory by id
        public async Task<Category?> GetOnlyCategoryByIdAsync(int id)
        {
            try
            {
                return await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == id && !(c.Status == (int)CategoryEnum.Status.DISABLE));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Categories
        public async Task<List<Category>> GetCategoriesAsync(string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, string type, int brandId, bool? isGetAll)
        {
            try
            {
                if (isGetAll != null && isGetAll == true)
                {
                    return await this._dbContext.Categories
                   .Include(x => x.Brand)
                   .Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                   .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync();
                }
                if (searchValue == null && searchValueWithoutUnicode is not null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Categories
                            .Include(x => x.Brand)
                            .Where(delegate (Category category)
                        {
                            if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }).Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                          .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                          .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                   then => then.OrderBy(x => x.Name))
                          .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                   then => then.OrderBy(x => x.Code))
                          .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                   then => then.OrderBy(x => x.DisplayOrder))
                          .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                then => then.OrderBy(x => x.Status).Reverse())
                          .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                          .ToList();
                    else if (sortByDESC is not null)
                        return this._dbContext.Categories
                            .Include(x => x.Brand)
                            .Where(delegate (Category category)
                        {
                            if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }).Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                          .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                          .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                   then => then.OrderByDescending(x => x.Name))
                          .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                                   then => then.OrderByDescending(x => x.Code))
                          .If(sortByDESC != null && sortByDESC.ToLower().Equals("type"),
                                   then => then.OrderByDescending(x => x.Type))
                          .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                                   then => then.OrderByDescending(x => x.DisplayOrder))
                          .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                then => then.OrderByDescending(x => x.Status).Reverse())
                          .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                          .ToList();

                    return this._dbContext.Categories
                        .Include(x => x.Brand)
                        .Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                          .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                          .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                          .ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return this._dbContext.Categories
                        .Include(x => x.Brand)
                        .Where(c => c.Name.ToLower().Contains(searchValue.ToLower()) && c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DISABLE))
                        .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                        .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                   then => then.OrderBy(x => x.Name))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                 then => then.OrderBy(x => x.Code))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("type"),
                                 then => then.OrderBy(x => x.Type))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                 then => then.OrderBy(x => x.DisplayOrder))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                then => then.OrderBy(x => x.Status).Reverse())
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                        .ToList();

                    else if (sortByDESC is not null)
                        return this._dbContext.Categories
                        .Include(x => x.Brand)
                        .Where(c => c.Name.ToLower().Contains(searchValue.ToLower()) && c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DISABLE))
                        .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                   then => then.OrderByDescending(x => x.Name))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                                 then => then.OrderByDescending(x => x.Code))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("type"),
                                 then => then.OrderByDescending(x => x.Type))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                                 then => then.OrderByDescending(x => x.DisplayOrder))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                then => then.OrderByDescending(x => x.Status).Reverse())
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                        .ToList();

                    return this._dbContext.Categories
                        .Include(x => x.Brand)
                        .Where(c => c.Name.ToLower().Contains(searchValue.ToLower()) && c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DISABLE))
                        .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                        .ToList();
                }

                if (sortByASC is not null)
                    return this._dbContext.Categories
                    .Include(x => x.Brand)
                    .Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                    .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                    .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                   then => then.OrderBy(x => x.Name))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                 then => then.OrderBy(x => x.Code))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("type"),
                                 then => then.OrderBy(x => x.Type))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                 then => then.OrderBy(x => x.DisplayOrder))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                then => then.OrderBy(x => x.Status).Reverse())
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                else if (sortByDESC is not null)
                    return this._dbContext.Categories
                        .Include(x => x.Brand)
                        .Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                    .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                            then => then.OrderByDescending(x => x.Name))
                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                            then => then.OrderByDescending(x => x.Code))
                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("type"),
                            then => then.OrderByDescending(x => x.Type))
                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                            then => then.OrderByDescending(x => x.DisplayOrder))
                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                then => then.OrderByDescending(x => x.Status).Reverse())
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return await this._dbContext.Categories
                    .Include(x => x.Brand)
                    .Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                    .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Create Category 
        public async Task CreateCategoryAsyncAsync(Category category)
        {
            try
            {
                await this._dbContext.Categories.AddAsync(category);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Update Category
        public void UpdateCategory(Category category)
        {
            try
            {
                this._dbContext.Entry<Category>(category).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number Categories
        public async Task<int> GetNumberCategoriesAsync(string? keySearchUniCode, string? keySearchNotUniCode, string type, int brandId)
        {
            try
            {
                if (keySearchUniCode == null && keySearchNotUniCode != null)
                {
                    return this._dbContext.Categories
                        .Include(c => c.Brand)
                        .Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Type.Equals(type.ToUpper()) && c.Brand.BrandId == brandId).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null)
                {
                    return await this._dbContext.Categories.Include(x => x.Brand).Where(c => c.Name.ToLower().Contains(keySearchUniCode.ToLower()) && !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Type.Equals(type.ToUpper()) && c.Brand.BrandId == brandId).CountAsync();
                }
                return await this._dbContext.Categories.Include(x => x.Brand).Where(c => !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Type.Equals(type.ToUpper()) && c.Brand.BrandId == brandId).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Search and Paging extra category
        public List<Category> SearchAndPagingExtraCategory(List<Category> categories, string? searchValue, string? searchValueWithoutUnicode,
            int currentPage, int itemsPerPage, string? sortByASC, string? sortByDESC, int brandId, bool? isGetAll)
        {
            try
            {
                if (isGetAll != null && isGetAll == true)
                {
                    return categories
                        .Where(c => !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                    .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToList();
                }
                if (searchValue == null && searchValueWithoutUnicode is not null)
                {
                    if (sortByASC is not null)
                        return categories.Where(delegate (Category category)
                        {
                            if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }).Where(c => !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                          .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                          .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                   then => then.OrderBy(x => x.Name))
                          .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                   then => then.OrderBy(x => x.Code))
                          .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                   then => then.OrderBy(x => x.DisplayOrder))
                          .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                then => then.OrderBy(x => x.Status).Reverse())
                          .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                          .ToList();
                    else if (sortByDESC is not null)
                        return categories.Where(delegate (Category category)
                        {
                            if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }).Where(c => !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                          .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                          .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                   then => then.OrderByDescending(x => x.Name))
                          .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                                   then => then.OrderByDescending(x => x.Code))
                          .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                                   then => then.OrderByDescending(x => x.DisplayOrder))
                          .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                then => then.OrderByDescending(x => x.Status).Reverse())
                          .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                          .ToList();

                    return categories.Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(searchValueWithoutUnicode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                          .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                          .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                          .ToList();
                }
                else if (searchValue != null && searchValueWithoutUnicode == null)
                {
                    if (sortByASC is not null)
                        return categories
                        .Where(c => c.Name.ToLower().Contains(searchValue.ToLower()) && !(c.Status == (int)CategoryEnum.Status.DISABLE))
                        .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                        .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                   then => then.OrderBy(x => x.Name))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                 then => then.OrderBy(x => x.Code))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                 then => then.OrderBy(x => x.DisplayOrder))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                then => then.OrderBy(x => x.Status).Reverse())
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                        .ToList();

                    else if (sortByDESC is not null)
                        return categories
                        .Where(c => c.Name.ToLower().Contains(searchValue.ToLower()) && !(c.Status == (int)CategoryEnum.Status.DISABLE))
                        .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                                   then => then.OrderByDescending(x => x.Name))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                                 then => then.OrderByDescending(x => x.Code))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                                 then => then.OrderByDescending(x => x.DisplayOrder))
                        .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                then => then.OrderByDescending(x => x.Status).Reverse())
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                        .ToList();

                    return categories
                        .Where(c => c.Name.ToLower().Contains(searchValue.ToLower()) && !(c.Status == (int)CategoryEnum.Status.DISABLE))
                        .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage)
                        .ToList();
                }

                if (sortByASC is not null)
                    return categories.Where(c => !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                    .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                    .If(sortByASC != null && sortByASC.ToLower().Equals("name"),
                                   then => then.OrderBy(x => x.Name))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("code"),
                                 then => then.OrderBy(x => x.Code))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("displayorder"),
                                 then => then.OrderBy(x => x.DisplayOrder))
                        .If(sortByASC != null && sortByASC.ToLower().Equals("status"),
                                then => then.OrderBy(x => x.Status).Reverse())
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                else if (sortByDESC is not null)
                    return categories.Where(c => !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                    .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("name"),
                            then => then.OrderByDescending(x => x.Name))
                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("code"),
                            then => then.OrderByDescending(x => x.Code))
                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("displayorder"),
                            then => then.OrderByDescending(x => x.DisplayOrder))
                   .If(sortByDESC != null && sortByDESC.ToLower().Equals("status"),
                                then => then.OrderByDescending(x => x.Status).Reverse())
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

                return categories.Where(c => !(c.Status == (int)CategoryEnum.Status.DISABLE) && c.Brand.BrandId == brandId)
                    .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                    .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Number ExtraCategories
        public int GetNumberExtraCategories(List<Category> categories, string? keySearchUniCode, string? keySearchNotUniCode, int brandId)
        {
            try
            {
                if (keySearchUniCode == null && keySearchNotUniCode != null)
                {
                    return categories.Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => c.Status != (int)CategoryEnum.Status.DISABLE && c.Brand.BrandId == brandId).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null)
                {
                    return categories.Where(c => c.Name.ToLower().Contains(keySearchUniCode.ToLower()) && c.Status != (int)CategoryEnum.Status.DISABLE && c.Brand.BrandId == brandId).Count();
                }
                return categories.Where(c => c.Status != (int)CategoryEnum.Status.DISABLE && c.Brand.BrandId == brandId).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public async Task<List<Category>> GetCategories(int storeId)
        {
            try
            {
                return await this._dbContext.Categories.Include(x => x.Products.Where(x => x.Status != (int)ProductEnum.Status.DISABLE)).ThenInclude(x => x.ChildrenProducts)
                                                       .Include(x => x.Brand).ThenInclude(x => x.Stores)
                                                       .Include(x => x.ExtraCategoryExtraCategoryNavigations)
                                                       .Include(x => x.Products.Where(x => x.Status != (int)ProductEnum.Status.DISABLE)).ThenInclude(x => x.PartnerProducts)
                                                       .Where(x => x.Brand.Stores.Any(s => s.StoreId == storeId) && x.Status != (int)CategoryEnum.Status.DISABLE).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Count number of type category by id brand
        public async Task<int> CountTypeCategoryNumberByBrandIdAsync(int brandId, CategoryEnum.Type type)
        {
            try
            {
                return await _dbContext.Categories.Where(c => c.Type.ToUpper().Equals(type.ToString())
                                                           && (c.Status == (int)CategoryEnum.Status.ACTIVE || c.Status == (int)CategoryEnum.Status.INACTIVE)
                                                           && c.Brand.BrandId == brandId).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
