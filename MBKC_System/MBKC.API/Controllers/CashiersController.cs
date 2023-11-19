using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Cashiers.Requests;
using MBKC.Service.DTOs.Cashiers.Responses;
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
    public class CashiersController : ControllerBase
    {
        private ICashierService _cashierService;
        private IValidator<CreateCashierRequest> _createCashierValidator;
        private IValidator<UpdateCashierRequest> _updateCashierValidator;
        private IValidator<UpdateCashierStatusRequest> _updateCashierStatusValidator;
        private IValidator<GetCashiersRequest> _getCashiersValidator;
        private IValidator<CashierRequest> _getCashierValidator;
        public CashiersController(ICashierService cashierService, IValidator<CreateCashierRequest> createCashierValidator,
            IValidator<UpdateCashierRequest> updateCashierValidator, IValidator<UpdateCashierStatusRequest> updateCashierStatusValidator,
            IValidator<GetCashiersRequest> getCashiersValidator, IValidator<CashierRequest> getCashierValidator)
        {
            this._cashierService = cashierService;
            this._createCashierValidator = createCashierValidator;
            this._updateCashierValidator = updateCashierValidator;
            this._updateCashierStatusValidator = updateCashierStatusValidator;
            this._getCashiersValidator = getCashiersValidator;
            this._getCashierValidator = getCashierValidator;
        }

        #region Get Cashiers
        /// <summary>
        /// Get Cashiers in the kitchen center.
        /// </summary>
        /// <param name="getCashiersRequest">The object contains SearchValue, ItemsPerPage, CurrentPage, SortBy</param>
        /// <returns>
        /// A list of Cashiers with requested conditions.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         searchValue = Bún đậu mắm tôm
        ///         currentPage = 1
        ///         itemsPerPage = 5
        ///         sortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        /// </remarks>
        /// <response code="200">Get list of cashiers successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetCashiersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager)]
        [HttpGet(APIEndPointConstant.Cashier.CashiersEndpoint)]
        public async Task<IActionResult> GetCashiersAsync([FromQuery] GetCashiersRequest getCashiersRequest)
        {
            ValidationResult validationResult = await this._getCashiersValidator.ValidateAsync(getCashiersRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetCashiersResponse getCashiersResponse = await this._cashierService.GetCashiersAsync(getCashiersRequest, claims);
            return Ok(getCashiersResponse);
        }
        #endregion

        #region Get Cashier
        /// <summary>
        /// Get a specific cashier by id.
        /// </summary>
        /// <param name="getCashierRequest">An object contains the cashier's id.</param>
        /// <returns>
        /// An object contains the cashier information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         id = 1
        /// </remarks>
        /// <response code="200">Get a specific cashier successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetCashierResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager, PermissionAuthorizeConstant.Cashier)]
        [HttpGet(APIEndPointConstant.Cashier.CashierEndpoint)]
        public async Task<IActionResult> GetCashierAsync([FromRoute] CashierRequest getCashierRequest)
        {
            ValidationResult validationResult = await this._getCashierValidator.ValidateAsync(getCashierRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetCashierResponse getCashierResponse = await this._cashierService.GetCashierAsync(getCashierRequest.Id, claims);
            return Ok(getCashierResponse);
        }
        #endregion

        #region Create cashier
        /// <summary>
        /// Create a new Cashier.
        /// </summary>
        /// <param name="createCashierRequest">The object contains created cashier information.</param>
        /// <returns>
        /// A success message about creating new cashier.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         fullName = Thái Quốc Toàn
        ///         gender = male | female
        ///         dateOfBirth = 2001/04/19
        ///         avatar = [File Image]
        ///         citizenNumber = 547865986321
        ///         email = example@gmail.com
        /// </remarks>
        /// <response code="200">Created new cashier successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager)]
        [HttpPost(APIEndPointConstant.Cashier.CashiersEndpoint)]
        public async Task<IActionResult> PostCreateCashierAsync([FromForm] CreateCashierRequest createCashierRequest)
        {
            ValidationResult validationResult = await this._createCashierValidator.ValidateAsync(createCashierRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._cashierService.CreateCashierAsync(createCashierRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.CashierMessage.CreatedCashierSuccessfully
            });
        }
        #endregion

        #region Update Cashier Information
        /// <summary>
        /// Update a specific cashier information.
        /// </summary>
        /// <param name="getCashierRequest">An object contains the cashier's id.</param>
        /// <param name="updateCashierRequest">The object contains updated cashier information.</param>
        /// <returns>
        /// A success message about updating specific cashier information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         fullName = Thái Quốc Toàn
        ///         gender = male | female
        ///         dateOfBirth = 2001/04/19
        ///         avatar = [File Image]
        ///         citizenNumber = 547865986321
        ///         newPassword = ********
        ///         Status = ACTIVE | INACTIVE
        /// </remarks>
        /// <response code="200">Updated cashier successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager, PermissionAuthorizeConstant.Cashier)]
        [HttpPut(APIEndPointConstant.Cashier.CashierEndpoint)]
        public async Task<IActionResult> UpdateCashierAsync([FromRoute] CashierRequest getCashierRequest, [FromForm] UpdateCashierRequest updateCashierRequest)
        {
            ValidationResult validationResultCashierId = await this._getCashierValidator.ValidateAsync(getCashierRequest);
            ValidationResult validationResult = await this._updateCashierValidator.ValidateAsync(updateCashierRequest);
            if (validationResultCashierId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultCashierId);
                throw new BadRequestException(errors);
            }
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._cashierService.UpdateCashierAsync(getCashierRequest.Id, updateCashierRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.CashierMessage.UpdatedCashierSuccessfully
            });
        }
        #endregion

        #region Update Cashier status
        /// <summary>
        /// Update a specific cashier status.
        /// </summary>
        /// <param name="getCashierRequest">The product's id.</param>
        /// <param name="updateCashierStatusRequest">The object contains updated product status.</param>
        /// <returns>
        /// A success message about updating specific cashier status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         {
        ///             Status: "ACTIVE | INACTIVE"
        ///         }
        /// </remarks>
        /// <response code="200">Updated cashier status successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager)]
        [HttpPut(APIEndPointConstant.Cashier.UpdatingCashierStatusEndpoint)]
        public async Task<IActionResult> UpdateCashierStatusAsync([FromRoute] CashierRequest getCashierRequest, [FromBody] UpdateCashierStatusRequest updateCashierStatusRequest)
        {
            ValidationResult validationResultCashierId = await this._getCashierValidator.ValidateAsync(getCashierRequest);
            ValidationResult validationResult = await this._updateCashierStatusValidator.ValidateAsync(updateCashierStatusRequest);
            if (validationResultCashierId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultCashierId);
                throw new BadRequestException(errors);
            }
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._cashierService.UpdateCashierStatusAsync(getCashierRequest.Id, updateCashierStatusRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.CashierMessage.UpdatedCashierStatusSuccessfully
            });
        }
        #endregion

        #region Delete Cashier
        /// <summary>
        /// Delete a specific cashier.
        /// </summary>
        /// <param name="getCashierRequest">An object contains the cashier's id.</param>
        /// <returns>
        /// A success message about deleting specific cashier.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         DELETE
        ///         id= 1
        /// </remarks>
        /// <response code="200">Deleted cashier successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager)]
        [HttpDelete(APIEndPointConstant.Cashier.CashierEndpoint)]
        public async Task<IActionResult> DeleteCashierAsync([FromRoute] CashierRequest getCashierRequest)
        {
            ValidationResult validationResult = await this._getCashierValidator.ValidateAsync(getCashierRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._cashierService.DeleteCashierAsync(getCashierRequest.Id, claims);
            return Ok(new
            {
                Message = MessageConstant.CashierMessage.DeletedCashierSuccessfully
            });
        }
        #endregion

        #region Get Cashier Report
        /// <summary>
        /// Get cashier report of a shift
        /// </summary>
        /// <returns>
        /// An object include information about cashier's shift that day.
        /// </returns>
        /// <response code="200">Get cashier report successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetCashierReportResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier)]
        [HttpGet(APIEndPointConstant.Cashier.CashierReportEndpoint)]
        public async Task<IActionResult> GetCashierReportAsync()
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getCashierReport = await this._cashierService.GetCashierReportAsync(claims);
            return Ok(getCashierReport);
        }
        #endregion
    }
}
