using MBKC.BAL.DTOs.Categories;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Products;
using MBKC.BAL.Errors;
using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MBKC.BAL.Utils;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Authorization;
using MBKC.BAL.Services.Interfaces;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ICategoryService _categoryService;
        private IOptions<FireBaseImage> _firebaseImageOptions;
        private IValidator<PostCategoryRequest> _postCategoryRequest;
        private IValidator<UpdateCategoryRequest> _updateCategoryRequest;
        public CategoriesController(ICategoryService categoryService,
            IValidator<PostCategoryRequest> postCategoryRequest,
            IValidator<UpdateCategoryRequest> updateCategoryRequest,
            IOptions<FireBaseImage> firebaseImageOptions)
        {
            this._categoryService = categoryService;
            this._firebaseImageOptions = firebaseImageOptions;
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
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [HttpPost]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> CreateCategoryAsync([FromForm] PostCategoryRequest postCategoryRequest)
        {
            ValidationResult validationResult = await this._postCategoryRequest.ValidateAsync(postCategoryRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            await this._categoryService.CreateCategoryAsync(postCategoryRequest, _firebaseImageOptions.Value);
            return Ok(new
            {
                Message = "Created Category Successfylly."
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
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [HttpPut("{id}")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> UpdateCategoryAsync([FromRoute] int id, [FromForm] UpdateCategoryRequest updateCategoryRequest)
        {
            ValidationResult validationResult = await this._updateCategoryRequest.ValidateAsync(updateCategoryRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            await this._categoryService.UpdateCategoryAsync(id, updateCategoryRequest, _firebaseImageOptions.Value);
            return Ok(new
            {
                Message = "Updated Category Successfully."
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
        /// <param name="pageNumber">
        ///  Page number user want to go.
        /// </param>
        /// <param name="pageSize">
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
        ///         pageNumber = 5
        ///         pageSize = 1
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
        [Produces("application/json")]
        [PermissionAuthorize("Brand Manager")]
        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync(string type, [FromQuery] string? keySearchName, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var data = await this._categoryService.GetCategoriesAsync(type, keySearchName, pageNumber, pageSize);

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
        [Produces("application/json")]
        [HttpGet("{id}")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] int id)
        {
            var data = await this._categoryService.GetCategoryByIdAsync(id);
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
        [Produces("application/json")]
        [HttpDelete("{id}")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> DeleteCategoryByIdAsync([FromRoute] int id)
        {
            await this._categoryService.DeActiveCategoryByIdAsync(id);
            return Ok(new
            {
                Message = "Deactive Category Successfully."
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
        /// <param name="pageNumber">
        ///  The current page the user wants to get next items.
        /// </param>
        /// <param name="pageSize">
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
        ///         pageNumber = 5
        ///         pageSize = 1
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
        [Produces("application/json")]
        [HttpGet("{id}/extra-categories")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> GetExtraCategoriesByCategoryId([FromRoute] int id, [FromQuery] string? keySearchName, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var data = await this._categoryService.GetExtraCategoriesByCategoryId(id, keySearchName, pageNumber, pageSize);
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
        /// <param name="listExtraCategoryId">
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
        ///            [2,3,4,5]
        ///         }
        ///         
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
        [Produces("application/json")]
        [HttpPost("{id}/add-extra-category")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> AddExtraCategoriesToNormalCategory([FromRoute] int id, [FromBody] List<int> listExtraCategoryId)
        {
            await this._categoryService.AddExtraCategoriesToNormalCategory(id, listExtraCategoryId);
            return Ok(new { Message = "Added Extra Category To Normal Category Successfully." });
        }
        #endregion

        // thiếu api lấy danh sách extra category

    }
}
