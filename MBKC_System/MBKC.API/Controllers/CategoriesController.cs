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
        private IValidator<PostCategoryRequest> _postCategoryRequest;
        private IValidator<UpdateCategoryRequest> _updateCategoryRequest;
        public CategoriesController(ICategoryService categoryService,
            IValidator<PostCategoryRequest> postCategoryRequest,
            IValidator<UpdateCategoryRequest> updateCategoryRequest)
        {
            this._categoryService = categoryService;
            this._postCategoryRequest = postCategoryRequest;
            this._updateCategoryRequest = updateCategoryRequest;
        }

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
        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpPost(APIEndPointConstant.Category.CategoriesEndpoint)]
        public async Task<IActionResult> CreateCategoryAsync([FromForm] PostCategoryRequest postCategoryRequest)
        {
            ValidationResult validationResult = await this._postCategoryRequest.ValidateAsync(postCategoryRequest);
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

        #region Update Category
        /// <summary>
        /// Update category by id.
        /// </summary>
        /// <param name="id">
        /// Category's id. 
        /// </param>
        ///  <param name="updateCategoryRequest">
        /// Object include information for update category
        ///  </param>
        /// <returns>
        /// A success message about updating category.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         Code = C001
        ///         Name = Thịt nguội
        ///         DisplayOrder = 3
        ///         Description = Thịt thêm vào bánh mỳ
        ///         ImgUrl = [Image file]
        ///         Status = Active | Inactive
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
        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpPut(APIEndPointConstant.Category.CategoryEndpoint)]
        public async Task<IActionResult> UpdateCategoryAsync([FromRoute] int id, [FromForm] UpdateCategoryRequest updateCategoryRequest)
        {
            ValidationResult validationResult = await this._updateCategoryRequest.ValidateAsync(updateCategoryRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            await this._categoryService.UpdateCategoryAsync(id, updateCategoryRequest, HttpContext);
            return Ok(new
            {
                Message = MessageConstant.CategoryMessage.UpdatedCategorySuccessfully
            });
        }
        #endregion

        #region Get Categories
        /// <summary>
        /// Get a list of categories from the system with condition paging, searchByName.
        /// </summary>
        /// <param name="type">
        ///  Include type of category are NORMAL or EXTRA
        /// </param>
        /// <param name="keySearchName">
        ///  The category name that the user wants to search.
        /// </param>
        /// <param name="currentPage">
        ///  Page number user want to go.
        /// </param>
        /// <param name="itemsPerPage">
        ///  Items user want display in 1 page.
        /// </param>
        /// <returns>
        /// A list of categories contains TotalItems, TotalPages, Categories's information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         type = NORMAL | EXTRA
        ///         keySearchName = Bánh
        ///         currentPage = 5
        ///         itemsPerPage = 1
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpGet(APIEndPointConstant.Category.CategoriesEndpoint)]
        public async Task<IActionResult> GetCategoriesAsync([FromQuery] string type, [FromQuery] string? keySearchName, [FromQuery] int? currentPage, [FromQuery] int? itemsPerPage)
        {
            var data = await this._categoryService.GetCategoriesAsync(type, keySearchName, currentPage, itemsPerPage, HttpContext);

            return Ok(data);
        }
        #endregion

        #region Get Category By Id
        /// <summary>
        ///  Get specific category by category Id.
        /// </summary>
        /// <param name="id">
        ///  Id of category.
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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpGet(APIEndPointConstant.Category.CategoryEndpoint)]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] int id)
        {
            var data = await this._categoryService.GetCategoryByIdAsync(id, HttpContext);
            return Ok(data);
        }
        #endregion

        #region Deleted Existed Category By Id
        /// <summary>
        ///  Delete existed category by id.
        /// </summary>
        /// <param name="id">
        ///  Id of category.
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpDelete(APIEndPointConstant.Category.CategoryEndpoint)]
        public async Task<IActionResult> DeleteCategoryByIdAsync([FromRoute] int id)
        {
            await this._categoryService.DeActiveCategoryByIdAsync(id, HttpContext);
            return Ok(new
            {
                Message = MessageConstant.CategoryMessage.DeletedCategorySuccessfully
            });
        }
        #endregion

        /*#region Get Products By Category Id
        /// <summary>
        /// Get products by category Id.
        /// </summary>
        /// <param name="id">
        ///  Id of category.
        /// </param>
        /// <param name="keySearchName">
        ///  The product name that the user wants to search.
        /// </param>
        /// <param name="pageNumber">
        ///  The current page the user wants to get next items.
        /// </param>
        /// <param name="pageSize">
        ///  Number of elements on a page.
        /// </param>
        /// <returns>
        /// A list of products contains TotalItems, TotalPages, products' information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 1
        ///         keySearchName = Bánh Quy Bơ
        ///         pageSize = 5
        ///         pageNumber = 1
        /// </remarks>
        /// <response code="200">Get products Successfully.</response>
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
        [Produces("application/json")]
        [HttpGet("{id}/products")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> GetProductsByCategoryIdAsync([FromRoute] int id, [FromQuery] string? keySearchName, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var data = await this._categoryService.GetProductsInCategory(id, keySearchName, pageNumber, pageSize);
            return Ok(data);
        }
        #endregion*/

        #region Get ExtraCategories By Category Id
        /// <summary>
        /// Get extraCategories by category Id.
        ///  </summary>
        /// <param name="id">
        ///  Id of category.
        /// </param>
        /// <param name="keySearchName">
        ///  The category name that the user wants to search.
        /// </param>
        /// <param name="currentPage">
        ///  The current page the user wants to get next items.
        /// </param>
        /// <param name="itemsPerPage">
        ///  Number of elements on a page.
        /// </param>
        /// <returns>
        ///  A list of categories contains TotalItems, TotalPages, product's information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 1
        ///         keySearchName = Ngò gai
        ///         currentPage = 5
        ///         itemsPerPage = 1
        ///        
        /// </remarks>
        /// <response code="200">Get Extra categories Successfully.</response>
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpGet(APIEndPointConstant.Category.ExtraCategoriesEndpoint)]
        public async Task<IActionResult> GetExtraCategoriesByCategoryId([FromRoute] int id, [FromQuery] string? keySearchName, [FromQuery] int? currentPage, [FromQuery] int? itemsPerPage)
        {
            var data = await this._categoryService.GetExtraCategoriesByCategoryId(id, keySearchName, currentPage, itemsPerPage, HttpContext);
            return Ok(data);
        }
        #endregion

        #region Add Extra Category To Normal Category
        /// <summary>
        ///  Add extra category to normal category.
        /// </summary>
        /// <param name="id">
        ///  Id of normal category.
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpPost(APIEndPointConstant.Category.ExtraCategoriesEndpoint)]
        public async Task<IActionResult> AddExtraCategoriesToNormalCategory([FromRoute] int id, [FromBody] ExtraCategoryRequest extraCategoryRequest)
        {
            await this._categoryService.AddExtraCategoriesToNormalCategory(id, extraCategoryRequest, HttpContext);
            return Ok(new { Message = MessageConstant.CategoryMessage.CreatedExtraCategoriesToNormalCategorySuccessfully });
        }
        #endregion
    }
}
