using MBKC.BAL.DTOs.Categories;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.Products;
using MBKC.BAL.Errors;
using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MBKC.BAL.Utils;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Authorization;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ICategoryRepository _categoryRepository;
        private IOptions<FireBaseImage> _firebaseImageOptions;
        private IValidator<PostCategoryRequest> _postCategoryRequest;
        private IValidator<UpdateCategoryRequest> _updateCategoryRequest;
        public CategoriesController(ICategoryRepository categoryRepository,
            IValidator<PostCategoryRequest> postCategoryRequest,
            IValidator<UpdateCategoryRequest> updateCategoryRequest,
            IOptions<FireBaseImage> firebaseImageOptions)
        {
            _categoryRepository = categoryRepository;
            _firebaseImageOptions = firebaseImageOptions;
            _postCategoryRequest = postCategoryRequest;
            _updateCategoryRequest = updateCategoryRequest;
        }
        #region Create Category
        /// <summary>
        /// Brand Manager create new category with type are NORMAL or EXTRA.
        /// </summary>
        /// <param name="postCategoryRequest">
        /// Include information about category for create new a category. 
        /// </param>
        /// <returns>
        /// An object will return CategoryId, Code, Name, Type, DisplayOrder, Description, ImgUrl, Status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         Code: BM988
        ///         Name: Bánh
        ///         Type: Normal
        ///         DisplayOrder: 1
        ///         Description: Bánh của hệ thống
        ///         ImgUrl: [Upload a Image file] 
        ///         Status: 1
        /// </remarks>
        /// <response code="200">Create category Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [HttpPost]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> CreateCategoryAsync([FromForm] PostCategoryRequest postCategoryRequest)
        {
            ValidationResult validationResult = await _postCategoryRequest.ValidateAsync(postCategoryRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._categoryRepository.CreateCategoryAsync(postCategoryRequest, _firebaseImageOptions.Value);
            return Ok(data);

        }
        #endregion

        #region Update Category
        /// <summary>
        /// Brand Manager update category by id.
        /// </summary>
        /// <param name="id">
        /// category's id for update category 
        /// </param>
        ///  <param name="updateCategoryRequest">
        /// Object include Name, DisplayOrder, Description, ImageUrl
        ///  </param>
        /// <returns>
        /// An Object will return CategoryId, Code, Name, Type, DisplayOrder, Description, ImgUrl, Status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         Name: Nước
        ///         DisplayOrder: 1
        ///         Description: Nước của hệ thống
        ///         ImageUrl: [Upload a Image file]
        /// </remarks>
        /// <response code="200">Update category Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpPut("{id}")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> UpdateCategoryAsync([FromRoute] int id, [FromForm] UpdateCategoryRequest updateCategoryRequest)
        {
            ValidationResult validationResult = await _updateCategoryRequest.ValidateAsync(updateCategoryRequest);
            if (!validationResult.IsValid)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            var data = await this._categoryRepository.UpdateCategoryAsync(id, updateCategoryRequest, _firebaseImageOptions.Value);
            return Ok(data);
        }
        #endregion

        #region Get Categories
        /// <summary>
        /// Brand Manager get Categories from the system and also paging and searchByName.
        /// </summary>
        /// <param name="searchCategoryRequest">
        ///  Include KeySearchName
        /// </param>
        /// <param name="PAGE_NUMBER">
        ///  Page number user want to go.
        /// </param>
        /// <param name="PAGE_SIZE">
        ///  Items user want display in 1 page.
        /// </param>
        /// <returns>
        /// An Object will return CategoryId, Code, Name, Type, DisplayOrder, Description, ImgUrl, Status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         KeySearchName: Bánh
        ///         PAGE_SIZE: 5
        ///         PAGE_NUMBER: 1
        /// </remarks>
        /// <response code="200">Get categories Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [PermissionAuthorize("Brand Manager")]
        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync(string type, [FromQuery] SearchCategoryRequest? searchCategoryRequest, [FromQuery] int? PAGE_NUMBER, [FromQuery] int? PAGE_SIZE)
        {
            var data = await this._categoryRepository.GetCategoriesAsync(type, searchCategoryRequest, PAGE_NUMBER, PAGE_SIZE);

            return Ok(new
            {
                categories = data.Item1,
                totalPage = data.Item2,
                pageNumber = data.Item3,
                pageSize = data.Item4
            });
        }
        #endregion

        #region Get Category By Id
        /// <summary>
        /// Brand Manager get Category by category Id.
        /// </summary>
        /// <param name="id">
        ///  Id of Category.
        /// </param>
        /// <returns>
        /// An Object will return CategoryId, Code, Name, Type, DisplayOrder, Description, ImgUrl, Status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id: 3
        /// </remarks>
        /// <response code="200">Get category Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpGet("{id}")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] int id)
        {
            var data = await this._categoryRepository.GetCategoryByIdAsync(id);
            return Ok(data);
        }
        #endregion

        #region Deactive Category By Id
        /// <summary>
        /// Brand manager Deactive Category by id.
        /// </summary>
        /// <param name="id">
        ///  Id of Category.
        /// </param>
        /// <returns>
        /// An Object will return CategoryId, Code, Name, Type, DisplayOrder, Description, ImgUrl, Status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         DELETE
        ///         id: 3
        /// </remarks>
        /// <response code="200">Deactive category successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpDelete("{id}")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> DeActiveCategoryByIdAsync([FromRoute] int id)
        {
            await this._categoryRepository.DeActiveCategoryByIdAsync(id);
            return Ok(new
            {
                Message = "Deactive category successfully"
            });
        }
        #endregion

        #region Get Products By category id
        /// <summary>
        /// Brand Manager get Products by category Id.
        /// </summary>
        /// <param name="id">
        ///  Id of Category.
        /// </param>
        /// <returns>
        /// An Object will return CategoryId, Code, Name, Type, DisplayOrder, Description, ImgUrl, Status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id: 1
        ///         KeySearchName: Bánh Quy Bơ
        ///         PAGE_SIZE: 5
        ///         PAGE_NUMBER: 1
        /// </remarks>
        /// <response code="200">Get products Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpGet("{id}/products")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> GetProductsByIdAsync([FromRoute] int id, [FromQuery] SearchProductsInCategory? searchProductsInCategory, [FromQuery] int? PAGE_NUMBER, [FromQuery] int? PAGE_SIZE)
        {
            var data = await this._categoryRepository.GetProductsInCategory(id, searchProductsInCategory, PAGE_NUMBER, PAGE_SIZE);

            return Ok(new
            {
                products = data.Item1,
                totalPage = data.Item2,
                pageNumber = data.Item3,
                pageSize = data.Item4
            });
        }
        #endregion

        #region Get ExtraCategories By category id
        /// <summary>
        /// Brand Manager get ExtraCategories by category Id.
        /// </summary>
        /// <param name="id">
        ///  Id of Category.
        /// </param>
        /// <returns>
        /// An Object will return CategoryId, Code, Name, Type, DisplayOrder, Description, ImgUrl, Status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id: 1
        ///         KeySearchName: Ngò gai
        ///         PAGE_SIZE: 5
        ///         PAGE_NUMBER: 1
        /// </remarks>
        /// <response code="200">Get Extra categories Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpGet("{id}/extra-categories")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> GetExtraCategoriesByCategoryId([FromRoute] int id, [FromQuery] SearchCategoryRequest? searchCategoryRequest, [FromQuery] int? PAGE_NUMBER, [FromQuery] int? PAGE_SIZE)
        {
            var data = await this._categoryRepository.GetExtraCategoriesByCategoryId(id, searchCategoryRequest, PAGE_NUMBER, PAGE_SIZE);

            return Ok(new
            {
                extraCategories = data.Item1,
                totalPage = data.Item2,
                pageNumber = data.Item3,
                pageSize = data.Item4
            });
        }
        #endregion

        #region Add extra category to normal category
        /// <summary>
        /// Brand Manager add extra category to normal category.
        /// </summary>
        /// <param name="id">
        ///  Id of Category.
        /// </param>
        /// <param name="extraCategoryId">
        ///  List extra categories user want to add to normal category.
        /// </param>
        /// <returns>
        /// Return message Add extra category to normal category successfully.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         id: 1
        ///         [2,3,4,5]
        /// </remarks>
        /// <response code="200">Add extra category to normal category successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpPost("{id}/add-extra-category")]
        [PermissionAuthorize("Brand Manager")]
        public async Task<IActionResult> AddExtraCategoriesToNormalCategory([FromRoute] int id, [FromBody] List<int> extraCategoryId)
        {
            await this._categoryRepository.AddExtraCategoriesToNormalCategory(id, extraCategoryId);
            return Ok(new { Message = "Add extra category to normal category successfully." });
        }
        #endregion
    }
}
