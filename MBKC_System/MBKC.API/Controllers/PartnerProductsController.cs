using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.PartnerProducts;
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
    public class PartnerProductsController : ControllerBase
    {
        private IPartnerProductService _PartnerProductService;
        private IValidator<PostPartnerProductRequest> _createPartnerProductValidator;
        private IValidator<UpdatePartnerProductRequest> _updatePartnerProductValidator;
        public PartnerProductsController(IPartnerProductService PartnerProductService,
            IValidator<UpdatePartnerProductRequest> updatePartnerProductValidator,
            IValidator<PostPartnerProductRequest> createPartnerProductValidator)
        {
            this._PartnerProductService = PartnerProductService;
            this._createPartnerProductValidator = createPartnerProductValidator;
            this._updatePartnerProductValidator = updatePartnerProductValidator;
        }
        #region Create Partner Product
        /// <summary>
        /// Create new Partner product.
        /// </summary>
        /// <param name="postPartnerProductRequest">A partner product object contains created information.</param>
        /// <returns>
        /// A success message about creating partner product information.
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
        /// <response code="200">Created new partner product successfully.</response>
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
        [HttpPost(APIEndPointConstant.PartnerProduct.PartnerProductsEndpoint)]
        public async Task<IActionResult> PostCreatePartnerProductAsync([FromBody] PostPartnerProductRequest postPartnerProductRequest)
        {
            ValidationResult validationResult = await this._createPartnerProductValidator.ValidateAsync(postPartnerProductRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._PartnerProductService.CreatePartnerProduct(postPartnerProductRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.PartnerProductMessage.CreatedPartnerProductSuccessfully
            });
        }
        #endregion

        #region Get a specific Partner Product
        /// <summary>
        /// Get a specific partner product by storeId, partnerId, productId.
        /// </summary>
        /// <param name="storeId">The store's id.</param>
        /// <param name="partnerId">The partner's id.</param>
        ///  <param name="productId">The product's id.</param>
        /// <returns>
        /// An object contains the partner product information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         productId = 2
        ///         partnerId = 1
        ///         storeId = 1
        /// </remarks>
        /// <response code="200">Get a specific partner product successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetPartnerProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.PartnerProduct.PartnerProductEndpoint)]
        public async Task<IActionResult> GetPartnerProductAsync([FromRoute] int productId, [FromRoute] int partnerId, [FromRoute] int storeId)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getPartnerProductResponse = await this._PartnerProductService.GetPartnerProduct(productId, partnerId, storeId, claims);
            return Ok(getPartnerProductResponse);
        }
        #endregion

        #region Get Partner Products
        /// <summary>
        /// Get Partner Products in the system.
        /// </summary>
        /// <param name="searchName">The name of product that user wants to find out.</param>
        /// <param name="currentPage">The number of page</param>
        /// <param name="itemsPerPage">The number of records that user wants to get.</param>
        /// <returns>
        /// A list of Partner products with requested conditions.
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
        [ProducesResponseType(typeof(GetPartnerProductsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.PartnerProduct.PartnerProductsEndpoint)]
        public async Task<IActionResult> GetPartnerProductsAsync([FromQuery] string? searchName, [FromQuery] int? currentPage, [FromQuery] int? itemsPerPage)

        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetPartnerProductsResponse getPartnerProductsResponse = await this._PartnerProductService.GetPartnerProducts(searchName, currentPage, itemsPerPage, claims);
            return Ok(getPartnerProductsResponse);
        }
        #endregion

        #region Update Existed Partner Product.
        /// <summary>
        /// Update existed partner product.
        /// </summary>
        /// <param name="storeId">The store's id.</param>
        /// <param name="partnerId">The partner's id.</param>
        ///  <param name="productId">The product's id.</param>
        /// <returns>
        /// A success message about updating partner product information.  
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
        /// <response code="200">Updated partner product information successfully.</response>
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
        [HttpPut(APIEndPointConstant.PartnerProduct.PartnerProductEndpoint)]
        public async Task<IActionResult> PutUpdatePartnerProductAsync([FromRoute] int productId, [FromRoute] int partnerId, [FromRoute] int storeId, [FromBody] UpdatePartnerProductRequest updatePartnerProductRequest)
        {
            ValidationResult validationResult = await this._updatePartnerProductValidator.ValidateAsync(updatePartnerProductRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._PartnerProductService.UpdatePartnerProduct(productId, partnerId, storeId, updatePartnerProductRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.PartnerProductMessage.UpdatedPartnerProductSuccessfully
            });
        }
        #endregion
    }
}
