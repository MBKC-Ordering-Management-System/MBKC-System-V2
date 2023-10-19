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
        public async Task<Category> GetCategoryByCodeAsync(string code)
        {
            try
            {
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Code.Equals(code));
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
                return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Name.Equals(name) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE));
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
                    .SingleOrDefaultAsync(c => c.CategoryId.Equals(id) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Get Categories
        public async Task<List<Category>> GetCategoriesAsync(string? keySearchNameUniCode, string? keySearchNameNotUniCode, string type, int? itemsPerPage, int? currentPage, int brandId)
        {
            try
            {
                if(itemsPerPage is null && currentPage is null)
                {
                    if (keySearchNameUniCode == null && keySearchNameNotUniCode != null)
                    {
                        return this._dbContext.Categories.Where(delegate (Category category)
                        {
                            if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNameNotUniCode.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }).Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE) && c.Brand.BrandId == brandId)
                          .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                          .ToList();
                    }
                    else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null)
                    {
                        return await this._dbContext.Categories
                            .Where(c => c.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE))
                            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                            .ToListAsync();
                    }
                    return await this._dbContext.Categories.Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE) && c.Brand.BrandId == brandId)
                        .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                        .ToListAsync();
                }
                if (keySearchNameUniCode == null && keySearchNameNotUniCode != null)
                {
                    return this._dbContext.Categories.Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNameNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE) && c.Brand.BrandId == brandId)
                      .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                      .Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToList();
                }
                else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null)
                {
                    return await this._dbContext.Categories
                        .Where(c => c.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE))
                        .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                        .Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
                }
                return await this._dbContext.Categories.Where(c => c.Type.Equals(type.ToUpper()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE) && c.Brand.BrandId == brandId)
                    .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
                    .Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToListAsync();
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
                    return this._dbContext.Categories.Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => !(c.Status == (int)CategoryEnum.Status.DEACTIVE) && c.Type.Equals(type.ToUpper()) && c.Brand.BrandId == brandId).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null)
                {
                    return await this._dbContext.Categories.Where(c => c.Name.ToLower().Contains(keySearchUniCode.ToLower()) && !(c.Status == (int)CategoryEnum.Status.DEACTIVE) && c.Type.Equals(type.ToUpper()) && c.Brand.BrandId == brandId).CountAsync();
                }
                return await this._dbContext.Categories.Where(c => !(c.Status == (int)CategoryEnum.Status.DEACTIVE) && c.Type.Equals(type.ToUpper()) && c.Brand.BrandId == brandId).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Search and Paging extra category
        public List<Category> SearchAndPagingExtraCategory(List<Category> categories, string? keySearchNameUniCode, string? keySearchNameNotUniCode, int itemsPerPage, int currentPage, int brandId)
        {
            try
            {
                if (keySearchNameUniCode == null && keySearchNameNotUniCode != null)
                {
                    return categories.Where(delegate (Category category)
                    {
                        if (StringUtil.RemoveSign4VietnameseString(category.Name.ToLower()).Contains(keySearchNameNotUniCode.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }).Where(c => c.Status != (int)CategoryEnum.Status.DEACTIVE && c.Brand.BrandId == brandId).Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                else if (keySearchNameUniCode != null && keySearchNameNotUniCode == null)
                {
                    return categories
                        .Where(c => c.Name.ToLower().Contains(keySearchNameUniCode.ToLower()) && c.Status != (int)CategoryEnum.Status.DEACTIVE && c.Brand.BrandId == brandId)
                        .Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();
                }
                return categories
                    .Where(c => c.Status != (int)CategoryEnum.Status.DEACTIVE && c.Brand.BrandId == brandId)
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
                    }).Where(c => c.Status != (int)CategoryEnum.Status.DEACTIVE && c.Brand.BrandId == brandId).AsQueryable().Count();
                }
                else if (keySearchUniCode != null && keySearchNotUniCode == null)
                {
                    return categories.Where(c => c.Name.ToLower().Contains(keySearchUniCode.ToLower()) && c.Status != (int)CategoryEnum.Status.DEACTIVE && c.Brand.BrandId == brandId).Count();
                }
                return categories.Where(c => c.Status != (int)CategoryEnum.Status.DEACTIVE && c.Brand.BrandId == brandId).Count();
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
                return await this._dbContext.Categories.Include(x => x.Products).ThenInclude(x => x.ChildrenProducts)
                                                       .Include(x => x.Brand).ThenInclude(x => x.Stores)
                                                       .Include(x => x.ExtraCategoryExtraCategoryNavigations)
                                                       .Where(x => x.Brand.Stores.Any(s => s.StoreId == storeId)).ToListAsync();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
