using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Products;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductService _productService;
        private IValidator<CreateProductRequest> _createProductValidator;
        private IValidator<UpdateProductRequest> _updateProductValidator;
        private IValidator<UpdateProductStatusRequest> _updateProductStatusValidator;
        public ProductsController(IProductService productService, IValidator<UpdateProductRequest> updateProductValidator,
            IValidator<CreateProductRequest> createProductValidator, IValidator<UpdateProductStatusRequest> updateProductStatusValidator)
        {
            this._productService = productService;
            this._updateProductValidator = updateProductValidator;
            this._createProductValidator = createProductValidator;
            _updateProductStatusValidator = updateProductStatusValidator;
        }

        #region Get Products
        /// <summary>
        /// Get Products in the system.
        /// </summary>
        /// <param name="searchName">The name of product that user wants to find out.</param>
        /// <param name="currentPage">The number of page</param>
        /// <param name="itemsPerPage">The number of records that user wants to get.</param>
        /// <param name="isGetAll">If user chooses TRUE option, currentPage and itemsPerPage are ignored. Default FALSE option.</param>
        /// <param name="idCategory">The category's id.</param>
        /// <param name="idStore">The store's id.</param>
        /// <param name="productType">The type of product such as: SINGLE, PARENT, CHILD, EXTRA.</param>
        /// <returns>
        /// A list of products with requested conditions.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         searchValue = Bún đậu mắm tôm
        ///         currentPage = 1
        ///         itemsPerPage = 5
        ///         productType = SINGLE | PARENT | CHILD | EXTRA
        ///         isGetALL = TRUE | FALSE
        ///         idCategory = 1
        ///         idStore = 1
        /// </remarks>
        /// <response code="200">Get list of products successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetProductsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager, PermissionAuthorizeConstant.StoreManager, 
                             PermissionAuthorizeConstant.KitchenCenterManager, PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpGet(APIEndPointConstant.Product.ProductsEndpoint)]
        public async Task<IActionResult> GetProductsAsync([FromQuery]string? searchName, [FromQuery] int? currentPage, 
            [FromQuery] int? itemsPerPage, [FromQuery] string? productType, [FromQuery] bool? isGetAll, [FromQuery] int? idCategory,
            [FromQuery] int? idStore)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetProductsResponse getProductsResponse = await this._productService.GetProductsAsync(searchName, currentPage, itemsPerPage, productType, isGetAll, idCategory, idStore, claims);
            return Ok(getProductsResponse);
        }
        #endregion

        #region Get a specific product
        /// <summary>
        /// Get a specific product by id.
        /// </summary>
        /// <param name="id">The product's id.</param>
        /// <returns>
        /// An object contains the product information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         id = 1
        /// </remarks>
        /// <response code="200">Get a specific product successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager, PermissionAuthorizeConstant.StoreManager, 
                             PermissionAuthorizeConstant.KitchenCenterManager, PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpGet(APIEndPointConstant.Product.ProductEndpoint)]
        public async Task<IActionResult> GetProductAsync([FromRoute] int id)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetProductResponse getProductResponse = await this._productService.GetProductAsync(id, claims);
            return Ok(getProductResponse);
        }
        #endregion

        #region Create new product
        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="createProductRequest">The object contains created product information.</param>
        /// <returns>
        /// A success message about creating new product.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         Code = BDMT0001
        ///         Name = Bún đậu mắm tôm
        ///         Description = Bún đậu mắm tôm thơn ngon
        ///         SellingPrice = 50000
        ///         DiscountPrice = 0
        ///         HistoricalPrice = 0
        ///         Size = S | M | L
        ///         Type = SINGLE | PARENT | CHILD | EXTRA
        ///         Image = [File Image]
        ///         DisplayOrder = 1
        ///         ParentProductId = 1
        ///         CategoryId = 1
        /// </remarks>
        /// <response code="200">Created new product successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpPost(APIEndPointConstant.Product.ProductsEndpoint)]
        public async Task<IActionResult> PostCreatNewProduct([FromForm]CreateProductRequest createProductRequest)
        {
            ValidationResult validationResult = await this._createProductValidator.ValidateAsync(createProductRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._productService.CreateProductAsync(createProductRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.ProductMessage.CreatedNewProductSuccessfully
            });
        }
        #endregion

        #region Update product information
        /// <summary>
        /// Update a specific product information.
        /// </summary>
        /// <param name="id">The product's id.</param>
        /// <param name="updateProductRequest">The object contains updated product information.</param>
        /// <returns>
        /// A success message about updating specific product information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         Name = Bún đậu mắm tôm
        ///         Description = Bún đậu mắm tôm thơn ngon
        ///         SellingPrice = 50000
        ///         DiscountPrice = 0
        ///         HistoricalPrice = 0
        ///         Image = [File Image]
        ///         DisplayOrder = 1
        ///         ParentProductId = 1
        ///         CategoryId = 1
        ///         Status = ACTIVE | INACTIVE
        /// </remarks>
        /// <response code="200">Updated product successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpPut(APIEndPointConstant.Product.ProductEndpoint)]
        public async Task<IActionResult> PutUpdateProductAsync([FromRoute] int id, [FromForm] UpdateProductRequest updateProductRequest)
        {
            ValidationResult validationResult = await this._updateProductValidator.ValidateAsync(updateProductRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._productService.UpdateProductAsync(id, updateProductRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.ProductMessage.UpdatedProductSuccessfully
            });
        }
        #endregion

        #region Update product status
        /// <summary>
        /// Update a specific product status.
        /// </summary>
        /// <param name="id">The product's id.</param>
        /// <param name="updateProductStatusRequest">The object contains updated product status.</param>
        /// <returns>
        /// A success message about updating specific product status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         {
        ///             Status: "ACTIVE | INACTIVE"
        ///         }
        /// </remarks>
        /// <response code="200">Updated product status successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpPut(APIEndPointConstant.Product.UpdatingStatusProductEndpoint)]
        public async Task<IActionResult> PutUpdateProductStatusAsync([FromRoute] int id, [FromBody] UpdateProductStatusRequest updateProductStatusRequest)
        {
            ValidationResult validationResult = await this._updateProductStatusValidator.ValidateAsync(updateProductStatusRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._productService.UpdateProductStatusAsync(id, updateProductStatusRequest, claims);
            return Ok( new
            {
                Message = MessageConstant.ProductMessage.UpdatedProductStatusSuccessfully
            });
        }
        #endregion

        #region Delete a product
        /// <summary>
        /// Delete a specific product.
        /// </summary>
        /// <param name="id">The product's id.</param>
        /// <returns>
        /// A success message about deleting specific product.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         DELETE
        ///         id= 1
        /// </remarks>
        /// <response code="200">Deleted product successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpDelete(APIEndPointConstant.Product.ProductEndpoint)]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] int id)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._productService.DeleteProductAsync(id, claims);
            return Ok(new
            {
                Message = MessageConstant.ProductMessage.DeletedProductSuccessfully
            });
        }
        #endregion
    }
}
