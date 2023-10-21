﻿using FluentValidation;
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
        
        private IPartnerProductService _partnerProductService;
        private IValidator<PostPartnerProductRequest> _createPartnerProductValidator;
        private IValidator<UpdatePartnerProductRequest> _updatePartnerProductValidator;
        private IValidator<UpdatePartnerProductStatusRequest> _updatePartnerProductStatusValidator;
        private IValidator<GetPartnerProductRequest> _getPartnerProductStatusValidator;
        public PartnerProductsController(IPartnerProductService partnerProductService,
            IValidator<UpdatePartnerProductRequest> updatePartnerProductValidator,
            IValidator<UpdatePartnerProductStatusRequest> updatePartnerProductStatusValidator,
            IValidator<GetPartnerProductRequest> getPartnerProductStatusValidator,
            IValidator<PostPartnerProductRequest> createPartnerProductValidator)
        {
            this._partnerProductService = partnerProductService;
            this._createPartnerProductValidator = createPartnerProductValidator;
            this._updatePartnerProductValidator = updatePartnerProductValidator;
            this._updatePartnerProductStatusValidator = updatePartnerProductStatusValidator;
            this._getPartnerProductStatusValidator= getPartnerProductStatusValidator;
        }
        #region Get Partner Products
        /// <summary>
        /// Get partner products in the system.
        /// </summary>
        /// <param name="getPartnerProductRequest">An object include SearchValue, ItemsPerPage, CurrentPage, SortBy for search, sort, paging.</param>
        /// <returns>
        /// A list of partner products with requested conditions.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         searchValue = Cơm bò xào
        ///         currentPage = 1
        ///         itemsPerPage = 5
        ///         sortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        /// </remarks>
        /// <response code="200">Get list of partner products successfully.</response>
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpGet(APIEndPointConstant.PartnerProduct.PartnerProductsEndpoint)]
        public async Task<IActionResult> GetPartnerProductsAsync([FromQuery] GetPartnerProductRequest getPartnerProductRequest)
        {
            ValidationResult validationResult = await this._getPartnerProductStatusValidator.ValidateAsync(getPartnerProductRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetPartnerProductsResponse getPartnerProductsResponse = await this._partnerProductService.GetPartnerProductsAsync(getPartnerProductRequest, claims);
            return Ok(getPartnerProductsResponse);
        }
        #endregion

        #region Get a specific partner product.
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpGet(APIEndPointConstant.PartnerProduct.PartnerProductEndpoint)]
        public async Task<IActionResult> GetPartnerProductAsync([FromRoute] int productId, [FromRoute] int partnerId, [FromRoute] int storeId)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getPartnerProductResponse = await this._partnerProductService.GetPartnerProductAsync(productId, partnerId, storeId, claims);
            return Ok(getPartnerProductResponse);
        }
        #endregion

        #region Create Partner Product
        /// <summary>
        /// Create new partner product.
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
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
            await this._partnerProductService.CreatePartnerProductAsync(postPartnerProductRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.PartnerProductMessage.CreatedPartnerProductSuccessfully
            });
        }
        #endregion

        #region Update Existed Partner Product.
        /// <summary>
        /// Update existed partner product.
        /// </summary>
        /// <param name="storeId">The store's id.</param>
        /// <param name="partnerId">The partner's id.</param>
        /// <param name="productId">The product's id.</param>
        /// <param name="updatePartnerProductRequest">Information to update partner product.</param>
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
        ///           {
        ///             "productCode": "ST001"
        ///             "status" : "INACTIVE"
        ///           }
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
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
            await this._partnerProductService.UpdatePartnerProductAsync(productId, partnerId, storeId, updatePartnerProductRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.PartnerProductMessage.UpdatedPartnerProductSuccessfully
            });
        }
        #endregion

        #region Update Existed Partner Product Status.
        /// <summary>
        /// Update status of existed partner product.
        /// </summary>
        /// <param name="storeId">The store's id.</param>
        /// <param name="partnerId">The partner's id.</param>
        /// <param name="productId">The product's id.</param>
        /// <param name="updatePartnerProductStatusRequest">An object include status to update partner product.</param>
        /// <returns>
        /// A success message about updating partner product status.  
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         storeId = 1
        ///         partnerId = 1
        ///         productId = 1
        ///           {
        ///             "status" : "INACTIVE"
        ///           }
        /// </remarks>
        /// <response code="200">Updated partner product status successfully.</response>
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
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpPut(APIEndPointConstant.PartnerProduct.UpdatingStatusPartnerProductEndpoint)]
        public async Task<IActionResult> PutUpdatePartnerProductStatusAsync([FromRoute] int productId, [FromRoute] int partnerId, [FromRoute] int storeId, [FromBody] UpdatePartnerProductStatusRequest updatePartnerProductStatusRequest)
        {
            ValidationResult validationResult = await this._updatePartnerProductStatusValidator.ValidateAsync(updatePartnerProductStatusRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._partnerProductService.UpdatePartnerProductStatusAsync(productId, partnerId, storeId, updatePartnerProductStatusRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.PartnerProductMessage.UpdatedPartnerProductStatusSuccessfully
            });
        }
        #endregion

        #region Delete existed Partner Product By Id
        /// <summary>
        /// Delete existed partner product by id.
        /// </summary>
        /// <param name="productId"> Id of product.</param>
        /// <param name="partnerId"> Id of partner.</param>
        /// <param name="storeId"> Id of store.</param>
        /// <returns>
        /// A success message about deleting partner product.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         DELETE
        ///             productId: 1
        ///             partnerId: 2
        ///             storeId: 1
        /// </remarks>
        /// <response code="200">Delete partner product successfully.</response>
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
        [HttpDelete(APIEndPointConstant.PartnerProduct.PartnerProductEndpoint)]
        public async Task<IActionResult> DeletePartnerProductByIdAsync([FromRoute] int productId, [FromRoute] int partnerId, [FromRoute] int storeId)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._partnerProductService.DeletePartnerProductByIdAsync(productId, partnerId, storeId, claims);
            return Ok(new
            {
                Message = MessageConstant.PartnerProductMessage.DeletedPartnerProductSuccessfully
            });
        }
        #endregion
    }
}
