using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.MappingProducts;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MBKC.API.Controllers
{

    [ApiController]
    public class MappingProductsController : ControllerBase
    {
        private IMappingProductService _mappingProductService;
        private IValidator<PostMappingProductRequest> _createMappingProductValidator;
        private IValidator<UpdateMappingProductRequest> _updateMappingProductValidator;
        public MappingProductsController(IMappingProductService mappingProductService,
            IValidator<UpdateMappingProductRequest> updateMappingProductValidator,
            IValidator<PostMappingProductRequest> createMappingProductValidator)
        {
            this._mappingProductService = mappingProductService;
            this._createMappingProductValidator = createMappingProductValidator;
            this._updateMappingProductValidator = updateMappingProductValidator;
        }
        #region Create Mapping Product
        /// <summary>
        /// Create new mapping product.
        /// </summary>
        /// <param name="postMappingProductRequest">A mapping product object contains created information.</param>
        /// <returns>
        /// A success message about creating mapping product information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "ProductId": "1"
        ///             "PartnerId": "2"
        ///             "StoreId": "2"
        ///             "ProductCode": "CT001"
        ///         }
        /// </remarks>
        /// <response code="200">Created new mapping product successfully.</response>
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
        [HttpPost(APIEndPointConstant.MappingProduct.MappingProductsEndpoint)]
        public async Task<IActionResult> PostCreateMappingProductAsync([FromBody] PostMappingProductRequest postMappingProductRequest)
        {
            ValidationResult validationResult = await this._createMappingProductValidator.ValidateAsync(postMappingProductRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._mappingProductService.CreateMappingProduct(postMappingProductRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.MappingProductMessage.CreatedMappingProductSuccessfully
            });
        }
        #endregion

        #region Get a specific Mapping Product
        /// <summary>
        /// Get a specific mapping product by storeId, partnerId, productId.
        /// </summary>
        /// <param name="storeId">The store's id.</param>
        /// <param name="partnerId">The partner's id.</param>
        ///  <param name="productId">The product's id.</param>
        /// <returns>
        /// An object contains the mapping product information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         productId = 2
        ///         partnerId = 1
        ///         storeId = 1
        /// </remarks>
        /// <response code="200">Get a specific mapping product successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetMappingProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.MappingProduct.MappingProductEndpoint)]
        public async Task<IActionResult> GetProductAsync([FromRoute] int productId, [FromRoute] int partnerId, [FromRoute] int storeId)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getMappingProductResponse = await this._mappingProductService.GetMappingProduct(productId, partnerId, storeId, claims);
            return Ok(getMappingProductResponse);
        }
        #endregion

        #region Get Mapping Products
        /// <summary>
        /// Get Mapping Products in the system.
        /// </summary>
        /// <param name="searchName">The name of product that user wants to find out.</param>
        /// <param name="currentPage">The number of page</param>
        /// <param name="itemsPerPage">The number of records that user wants to get.</param>
        /// <returns>
        /// A list of mapping products with requested conditions.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         searchValue = Cơm bò xào
        ///         currentPage = 1
        ///         itemsPerPage = 5
        /// </remarks>
        /// <response code="200">Get list of mapping products successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetMappingProductsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.MappingProduct.MappingProductsEndpoint)]
        public async Task<IActionResult> GetMappingProductsAsync([FromQuery] string? searchName, [FromQuery] int? currentPage, [FromQuery] int? itemsPerPage)

        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetMappingProductsResponse getMappingProductsResponse = await this._mappingProductService.GetMappingProducts(searchName, currentPage, itemsPerPage, claims);
            return Ok(getMappingProductsResponse);
        }
        #endregion

        #region Update Existed Mapping Product.
        /// <summary>
        /// Update product code of Mapping Product.
        /// </summary>
        /// <param name="storeId">The store's id.</param>
        /// <param name="partnerId">The partner's id.</param>
        ///  <param name="productId">The product's id.</param>
        /// <returns>
        /// A success message about updating mapping product information.  
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         storeId = 1
        ///         partnerId = 1
        ///         productId = 1
        ///         {
        ///             "ProductCode": "ST001"
        ///         }
        /// </remarks>
        /// <response code="200">Updated mapping product information successfully.</response>
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
        [HttpPut(APIEndPointConstant.MappingProduct.MappingProductEndpoint)]
        public async Task<IActionResult> PutUpdateStoreAsync([FromRoute] int productId, [FromRoute] int partnerId, [FromRoute] int storeId, [FromBody] UpdateMappingProductRequest updateMappingProductRequest)
        {
            ValidationResult validationResult = await this._updateMappingProductValidator.ValidateAsync(updateMappingProductRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._mappingProductService.UpdateMappingProduct(productId, partnerId, storeId, updateMappingProductRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.MappingProductMessage.UpdatedMappingProductSuccessfully
            });
        }
        #endregion
    }
}
