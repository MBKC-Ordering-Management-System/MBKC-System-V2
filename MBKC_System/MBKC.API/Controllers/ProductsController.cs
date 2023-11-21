using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Products;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Mvc;
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
        private IValidator<GetProductsRequest> _getProductsValidator;
        private IValidator<GetProductWithNumberSoldRequest> _getProductsWithNumberSoldValidator;
        private IValidator<ProductRequest> _getProductValidator;
        public ProductsController(IProductService productService, 
            IValidator<UpdateProductRequest> updateProductValidator,
            IValidator<ProductRequest> getProductValidator,
            IValidator<GetProductsRequest> getProductsValidator,
            IValidator<CreateProductRequest> createProductValidator,
            IValidator<UpdateProductStatusRequest> updateProductStatusValidator,
            IValidator<GetProductWithNumberSoldRequest> getProductsWithNumberSoldValidator)
        {
            this._productService = productService;
            this._updateProductValidator = updateProductValidator;
            this._createProductValidator = createProductValidator;
            this._updateProductStatusValidator = updateProductStatusValidator;
            this._getProductsValidator = getProductsValidator;
            this._getProductValidator = getProductValidator;
            _getProductsWithNumberSoldValidator = getProductsWithNumberSoldValidator;
        }

        #region Get Products
        /// <summary>
        /// Get Products in the system.
        /// </summary>
        /// <param name="getProductsRequest">An object include Search Value, ItemsPerPage, CurrentPage,
        /// SortBy, ProductType, IdCategory, IdStore, IsGetAll.
        /// </param>
        /// <returns>
        /// A list of products with requested conditions.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         searchValue = Bún đậu mắm tôm
        ///         itemsPerPage = 5
        ///         currentPage = 1
        ///         sortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        ///         productType = SINGLE | PARENT | CHILD | EXTRA
        ///         idCategory = 1
        ///         idStore = 1
        ///         isGetAll = TRUE | FALSE
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
        public async Task<IActionResult> GetProductsAsync([FromQuery] GetProductsRequest getProductsRequest)
        {
            ValidationResult validationResult = await this._getProductsValidator.ValidateAsync(getProductsRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetProductsResponse getProductsResponse = await this._productService.GetProductsAsync(getProductsRequest, claims);
            return Ok(getProductsResponse);
        }
        #endregion

        #region Get product with number of product sold
        /// <summary>
        /// Get Products with number of product sold in the system.
        /// </summary>
        /// <param name="getProductsRequest">An object include Search Value, ItemsPerPage, CurrentPage,
        /// SortBy, ProductType, IdCategory, IdStore, IsGetAll.
        /// </param>
        /// <returns>
        /// A list of products with number of product sold with requested conditions.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         searchValue = Bún đậu mắm tôm
        ///         itemsPerPage = 5
        ///         currentPage = 1
        ///         sortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        ///         productType = SINGLE | CHILD 
        ///         idCategory = 1
        ///         idStore = 1
        ///         isGetAll = TRUE | FALSE
        /// </remarks>
        /// <response code="200">Get list of products with number of product sold successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.Product.ProductWithNumberSoldEndpoint)]
        public async Task<IActionResult> GetProductsWithNumberOfProductSoldAsync([FromQuery] GetProductWithNumberSoldRequest getProductsRequest)
        {
            ValidationResult validationResult = await this._getProductsWithNumberSoldValidator.ValidateAsync(getProductsRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetProductsResponse getProductsResponse = await this._productService.GetProductsWithNumberOfProductSoldAsync(getProductsRequest, claims);
            return Ok(getProductsResponse);
        }
        #endregion

        #region Get a specific product
        /// <summary>
        /// Get a specific product by id.
        /// </summary>
        /// <param name="getProductRequest">An object include id of product.</param>
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
        public async Task<IActionResult> GetProductAsync([FromRoute] ProductRequest getProductRequest)
        {
            ValidationResult validationResult = await this._getProductValidator.ValidateAsync(getProductRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetProductResponse getProductResponse = await this._productService.GetProductAsync(getProductRequest.Id, claims);
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
        public async Task<IActionResult> PostCreatNewProductAsync([FromForm] CreateProductRequest createProductRequest)
        {
            ValidationResult validationResult = await this._createProductValidator.ValidateAsync(createProductRequest);
            if (validationResult.IsValid == false)
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
        /// <param name="getProductRequest">An object include id of product.</param>
        /// <param name="updateProductRequest">The object contains updated product information.</param>
        /// <returns>
        /// A success message about updating specific product information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT
        ///         Id = 1
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
        public async Task<IActionResult> PutUpdateProductAsync([FromRoute] ProductRequest getProductRequest, [FromForm] UpdateProductRequest updateProductRequest)
        {
            ValidationResult validationResult = await this._updateProductValidator.ValidateAsync(updateProductRequest);
            ValidationResult validationResultProductId = await this._getProductValidator.ValidateAsync(getProductRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            if (validationResultProductId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultProductId);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._productService.UpdateProductAsync(getProductRequest.Id, updateProductRequest, claims);
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
        /// <param name="getProductRequest">An object include id of product.</param>
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
        public async Task<IActionResult> PutUpdateProductStatusAsync([FromRoute] ProductRequest getProductRequest, [FromBody] UpdateProductStatusRequest updateProductStatusRequest)
        {
            ValidationResult validationResult = await this._updateProductStatusValidator.ValidateAsync(updateProductStatusRequest);
            ValidationResult validationResultProductId = await this._getProductValidator.ValidateAsync(getProductRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            if (validationResultProductId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultProductId);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._productService.UpdateProductStatusAsync(getProductRequest.Id, updateProductStatusRequest, claims);
            return Ok(new
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
