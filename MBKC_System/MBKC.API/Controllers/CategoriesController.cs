using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Categories;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ICategoryService _categoryService;
        private IValidator<PostCategoryRequest> _postCategoryValidator;
        private IValidator<UpdateCategoryRequest> _updateCategoryValidator;
        private IValidator<GetCategoriesRequest> _getCategoriesValidator;
        private IValidator<CategoryRequest> _getCategoryValidator;
        private IValidator<GetExtraCategoriesRequest> _getExtraCategoriesValidator;
        public CategoriesController(ICategoryService categoryService,
            IValidator<PostCategoryRequest> postCategoryValidator,
            IValidator<GetCategoriesRequest> getCategoriesValidator,
            IValidator<CategoryRequest> getCategoryValidator,
            IValidator<GetExtraCategoriesRequest> getExtraCategoriesValidator,
            IValidator<UpdateCategoryRequest> updateCategoryValidator)
        {
            this._categoryService = categoryService;
            this._postCategoryValidator = postCategoryValidator;
            this._updateCategoryValidator = updateCategoryValidator;
            this._getCategoriesValidator = getCategoriesValidator;
            this._getCategoryValidator = getCategoryValidator;
            this._getExtraCategoriesValidator = getExtraCategoriesValidator;
        }
        #region Get Categories
        /// <summary>
        /// Get a list of categories from the system.
        /// </summary>
        /// <param name="getCategoriesRequest">
        ///  An object incluce SearchValue, ItemsPerpage, CurrentPage, SortBy, Type for search, sort and paging.
        /// </param>
        /// <returns>
        /// A list of categories contains TotalItems, TotalPages, Categories's information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         Type = NORMAL | EXTRA
        ///         SearchValue = Bánh mỳ que
        ///         ItemsPerPage = 1
        ///         CurrentPage = 5
        ///         SortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        ///        
        /// </remarks>
        /// <response code="200">Get categories Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetCategoriesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager, PermissionAuthorizeConstant.StoreManager)]
        [HttpGet(APIEndPointConstant.Category.CategoriesEndpoint)]
        public async Task<IActionResult> GetCategoriesAsync([FromQuery] GetCategoriesRequest getCategoriesRequest)
        {
            ValidationResult validationResult = await this._getCategoriesValidator.ValidateAsync(getCategoriesRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._categoryService.GetCategoriesAsync(getCategoriesRequest, HttpContext);

            return Ok(data);
        }
        #endregion

        #region Get Category By Id
        /// <summary>
        ///  Get specific category by category Id.
        /// </summary>
        /// <param name="getCategoryRequest">
        /// An object include id of category.
        /// </param>
        /// <returns>
        /// An object contains category's information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 3
        /// </remarks>
        /// <response code="200">Get category Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager, PermissionAuthorizeConstant.StoreManager)]
        [HttpGet(APIEndPointConstant.Category.CategoryEndpoint)]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] CategoryRequest getCategoryRequest)
        {
            ValidationResult validationResult = await this._getCategoryValidator.ValidateAsync(getCategoryRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._categoryService.GetCategoryByIdAsync(getCategoryRequest.Id, HttpContext);
            return Ok(data);
        }
        #endregion

        #region Get ExtraCategories By Category Id
        /// <summary>
        /// Get extraCategories by category id.
        ///  </summary>
        /// <param name="getCategoryRequest">
        ///  An object include id of category.
        /// </param>
        /// <param name="getExtraCategoriesRequest">
        ///  An object incluce SearchValue, ItemsPerpage, CurrentPage, SortBy, Type for search, sort and paging.
        /// </param>
        /// <returns>
        ///  A list of categories contains TotalItems, TotalPages, extra categories information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         SearchValue = Bánh mỳ que
        ///         ItemsPerPage = 1
        ///         CurrentPage = 5
        ///         SortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        ///         isGetAll = True | False
        ///        
        /// </remarks>
        /// <response code="200">Get Extra categories Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetCategoriesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.Category.ExtraCategoriesEndpoint)]
        public async Task<IActionResult> GetExtraCategoriesByCategoryId([FromRoute] CategoryRequest getCategoryRequest, [FromQuery] GetExtraCategoriesRequest getExtraCategoriesRequest)
        {
            ValidationResult validationResultCategoryId = await this._getCategoryValidator.ValidateAsync(getCategoryRequest);
            ValidationResult validationResult = await this._getExtraCategoriesValidator.ValidateAsync(getExtraCategoriesRequest);
            if (!validationResultCategoryId.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResultCategoryId);
                throw new BadRequestException(error);
            }
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._categoryService.GetExtraCategoriesByCategoryId(getCategoryRequest.Id, getExtraCategoriesRequest, HttpContext);
            return Ok(data);
        }
        #endregion

        #region Create Category
        /// <summary>
        /// Create new category with type are NORMAL or EXTRA.
        /// </summary>
        /// <param name="postCategoryRequest">
        /// An object include information about category.
        /// </param>
        /// <returns>
        /// A success message about creating new category.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         
        ///         Code = BM988
        ///         Name = Bánh
        ///         Type = Normal | Extra
        ///         DisplayOrder = 1
        ///         Description = Bánh của hệ thống
        ///         ImgUrl = [Image file]
        /// </remarks>
        /// <response code="200">Created Category Successfylly.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpPost(APIEndPointConstant.Category.CategoriesEndpoint)]
        public async Task<IActionResult> CreateCategoryAsync([FromForm] PostCategoryRequest postCategoryRequest)
        {
            ValidationResult validationResult = await this._postCategoryValidator.ValidateAsync(postCategoryRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            await this._categoryService.CreateCategoryAsync(postCategoryRequest, HttpContext);
            return Ok(new
            {
                Message = MessageConstant.CategoryMessage.CreatedNewCategorySuccessfully
            });
        }
        #endregion

        #region Add Extra Category To Normal Category
        /// <summary>
        ///  Add extra category to normal category.
        /// </summary>
        /// <param name="categoryRequest">
        ///  An object include id of normal category.
        /// </param>
        /// <param name="extraCategoryRequest">
        ///  List extra categories user want to add to normal category.
        /// </param>
        /// <returns>
        /// Return message Add extra category to normal category successfully.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         id = 1
        /// 
        ///         {
        ///           "extraCategoryIds": [
        ///                        4,5,6
        ///                     ]
        ///         }
        /// </remarks>
        /// <response code="200">Add Extra Category To Normal Category Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpPost(APIEndPointConstant.Category.ExtraCategoriesEndpoint)]
        public async Task<IActionResult> AddExtraCategoriesToNormalCategoryAsync([FromRoute] CategoryRequest categoryRequest, [FromBody] ExtraCategoryRequest extraCategoryRequest)
        {
            ValidationResult validationResult = await this._getCategoryValidator.ValidateAsync(categoryRequest);

            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            await this._categoryService.AddExtraCategoriesToNormalCategoryAsync(categoryRequest.Id, extraCategoryRequest, HttpContext);
            return Ok(new { Message = MessageConstant.CategoryMessage.CreatedExtraCategoriesToNormalCategorySuccessfully });
        }
        #endregion

        #region Update Category
        /// <summary>
        /// Update category by id.
        /// </summary>
        /// <param name="getCategoryRequest">
        /// An object include id of category. 
        /// </param>
        ///  <param name="updateCategoryRequest">
        /// Object include information for update category.
        ///  </param>
        /// <returns>
        /// A success message about updating category.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         id = 3
        ///         Code = C001
        ///         Name = Thịt nguội
        ///         DisplayOrder = 3
        ///         Description = Thịt thêm vào bánh mỳ
        ///         ImgUrl = [Image file]
        ///         Status = ACTIVE | INACTIVE
        /// </remarks>
        /// <response code="200">Updated Category Successfully.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpPut(APIEndPointConstant.Category.CategoryEndpoint)]
        public async Task<IActionResult> UpdateCategoryAsync([FromRoute] CategoryRequest getCategoryRequest, [FromForm] UpdateCategoryRequest updateCategoryRequest)
        {
            ValidationResult validationResult = await this._updateCategoryValidator.ValidateAsync(updateCategoryRequest);
            ValidationResult validationResultCategoryId = await this._getCategoryValidator.ValidateAsync(getCategoryRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            if (!validationResultCategoryId.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResultCategoryId);
                throw new BadRequestException(error);
            }
            await this._categoryService.UpdateCategoryAsync(getCategoryRequest.Id, updateCategoryRequest, HttpContext);
            return Ok(new
            {
                Message = MessageConstant.CategoryMessage.UpdatedCategorySuccessfully
            });
        }
        #endregion

        #region Deleted Existed Category By Id
        /// <summary>
        ///  Delete existed category by id.
        /// </summary>
        /// <param name="getCategoryRequest">
        ///  An object include id of category.
        /// </param>
        /// <returns>
        /// A sucess message about deleting existed category.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         DELETE
        ///         id = 3
        ///         
        /// </remarks>
        /// <response code="200">Deleted existed category successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpDelete(APIEndPointConstant.Category.CategoryEndpoint)]
        public async Task<IActionResult> DeleteCategoryByIdAsync([FromRoute] CategoryRequest getCategoryRequest)
        {
            ValidationResult validationResult = await this._getCategoryValidator.ValidateAsync(getCategoryRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            await this._categoryService.DeActiveCategoryByIdAsync(getCategoryRequest.Id, HttpContext);
            return Ok(new
            {
                Message = MessageConstant.CategoryMessage.DeletedCategorySuccessfully
            });
        }
        #endregion
    }
}
