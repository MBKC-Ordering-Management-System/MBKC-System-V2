using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs;
using MBKC.Service.DTOs.BankingAccounts;
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
    public class BankingAccountsController : ControllerBase
    {
        private IBankingAccountService _bankingAccountService;
        private IValidator<CreateBankingAccountRequest> _createBankingAccountValidator;
        private IValidator<UpdateBankingAccountStatusRequest> _updateBankingAccountStatusValidator;
        private IValidator<UpdateBankingAccountRequest> _updateBankingAccountValidator;
        private IValidator<GetBankingAccountsRequest> _getBankingAccountsValidator;
        private IValidator<BankingAccountRequest> _getBankingAccountValidator;
        public BankingAccountsController(IBankingAccountService bankingAccountService,
            IValidator<CreateBankingAccountRequest> createBankingAccountValidator,
            IValidator<UpdateBankingAccountStatusRequest> updateBankingAccountStatusValidator,
            IValidator<GetBankingAccountsRequest> getBankingAccountsValidator,
            IValidator<BankingAccountRequest> getBankingAccountValidator,
            IValidator<UpdateBankingAccountRequest> updateBankingAccountValidator)
        {
            this._bankingAccountService = bankingAccountService;
            this._createBankingAccountValidator = createBankingAccountValidator;
            this._updateBankingAccountStatusValidator = updateBankingAccountStatusValidator;
            this._updateBankingAccountValidator = updateBankingAccountValidator;
            this._getBankingAccountsValidator = getBankingAccountsValidator;
            this._getBankingAccountValidator = getBankingAccountValidator;
        }

        #region Get banking accounts of a kitchen center 
        /// <summary>
        ///  Get a list of banking accounts from the system with condition paging, searchByName, sort.
        /// </summary>
        /// <param name="getBankingAccountsRequest">
        /// An object include Search Value, ItemsPerPage, CurrentPage, SortBy.
        /// </param>
        /// <returns>
        /// A list of banking accounts contains NumberItems, TotalPages, banking account' information
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         searchValue = Momo
        ///         currentPage = 1
        ///         itemsPerPage = 5
        ///         sortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        /// </remarks>
        /// <response code="200">Get banking accounts Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetBankingAccountsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager, PermissionAuthorizeConstant.Cashier)]
        [HttpGet(APIEndPointConstant.BankingAccount.BankingAccountsEndpoint)]
        public async Task<IActionResult> GetBankingAccountsAsync([FromQuery] GetBankingAccountsRequest getBankingAccountsRequest)
        {
            ValidationResult validationResult = await this._getBankingAccountsValidator.ValidateAsync(getBankingAccountsRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetBankingAccountsResponse getBankingAccountsResponse = await this._bankingAccountService.GetBankingAccountsAsync(getBankingAccountsRequest, claims);
            return Ok(getBankingAccountsResponse);
        }
        #endregion

        #region Get a specific banking account
        /// <summary>
        /// Get a specific banking account information of a kitchen center
        /// </summary>
        /// <param name="getBankingAccountRequest">An object include id of banking account.</param>
        /// <returns>
        /// An object contains banking account information.
        /// </returns>
        ///<remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         id = 1
        /// </remarks>
        /// <response code="200">Get banking account information of a kitchen center Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetBankingAccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager)]
        [HttpGet(APIEndPointConstant.BankingAccount.BankingAccountEndpoint)]
        public async Task<IActionResult> GetBankingAccountAsync([FromRoute] BankingAccountRequest getBankingAccountRequest)
        {
            ValidationResult validationResult = await this._getBankingAccountValidator.ValidateAsync(getBankingAccountRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetBankingAccountResponse getBankingAccountResponse = await this._bankingAccountService.GetBankingAccountAsync(getBankingAccountRequest.Id, claims);
            return Ok(getBankingAccountResponse);
        }
        #endregion

        #region Create new banking account
        /// <summary>
        /// Create new banking account.
        /// </summary>
        /// <param name="bankingAccountRequest">An object contains banking account information.</param>
        /// <returns>
        /// A success message about creating new banking account.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         
        ///         BankName = MoMo
        ///         BankLogo = [File image]
        ///         NumberAccount = 0886448660
        /// </remarks>
        /// <response code="200">Created new banking account Successfully.</response>
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
        [HttpPost(APIEndPointConstant.BankingAccount.BankingAccountsEndpoint)]
        public async Task<IActionResult> PostCreateBankingAccountAsync([FromForm] CreateBankingAccountRequest bankingAccountRequest)
        {
            ValidationResult validationResult = await this._createBankingAccountValidator.ValidateAsync(bankingAccountRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._bankingAccountService.CreateBankingAccountAsync(bankingAccountRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.BankingAccountMessage.CreatedNewBankingAccountSuccessfully
            });
        }
        #endregion

        #region Update banking account information
        /// <summary>
        /// Update banking account information.
        /// </summary>
        /// <param name="getBankingAccountRequest">An object include id of banking account.</param>
        /// <param name="bankingAccountRequest">An object contains banking account information.</param>
        /// <returns>
        /// A success message about updating banking account information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         
        ///         id = 1
        ///         BankName = MoMo
        ///         BankLogo = [File image]
        ///         status = ACTIVE | INACTIVE
        /// </remarks>
        /// <response code="200">Updated banking account information Successfully.</response>
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
        [HttpPut(APIEndPointConstant.BankingAccount.BankingAccountEndpoint)]
        public async Task<IActionResult> PutUpdateBankingAccountAsync([FromRoute] BankingAccountRequest getBankingAccountRequest, [FromForm] UpdateBankingAccountRequest bankingAccountRequest)
        {
            ValidationResult validationResult = await this._updateBankingAccountValidator.ValidateAsync(bankingAccountRequest);
            ValidationResult validationResultBankingAccountId = await this._getBankingAccountValidator.ValidateAsync(getBankingAccountRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            if (validationResultBankingAccountId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultBankingAccountId);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._bankingAccountService.UpdateBankingAccountAsync(getBankingAccountRequest.Id, bankingAccountRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.BankingAccountMessage.UpdatedBankingAccountSuccessfully
            });
        }
        #endregion

        #region Update banking account status
        /// <summary>
        /// Update banking account status.
        /// </summary>
        /// <param name="getBankingAccountRequest">An object include id of banking account.</param>
        /// <param name="bankingAccountStatusRequest">An object contains status property.</param>
        /// <returns>
        /// A success message about updating banking account status.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         PUT
        ///         
        ///         id = 1
        ///         
        ///         {
        ///             "status": "ACTIVE | INACTIVE"
        ///         }
        /// </remarks>
        /// <response code="200">Updated banking account status Successfully.</response>
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
        [HttpPut(APIEndPointConstant.BankingAccount.UpdatingStatusBankingAccountEndpoint)]
        public async Task<IActionResult> PutUpdateBankingAccountStatusAsync([FromRoute] BankingAccountRequest getBankingAccountRequest, [FromBody] UpdateBankingAccountStatusRequest bankingAccountStatusRequest)
        {
            ValidationResult validationResult = await this._updateBankingAccountStatusValidator.ValidateAsync(bankingAccountStatusRequest);
            ValidationResult validationResultBankingAccountId = await this._getBankingAccountValidator.ValidateAsync(getBankingAccountRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            if (validationResultBankingAccountId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultBankingAccountId);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._bankingAccountService.UpdateBankingAccountStatusAsync(getBankingAccountRequest.Id, bankingAccountStatusRequest, claims);
            return Ok(new
            {
                Message = MessageConstant.BankingAccountMessage.UpdatedStatusBankingAccountSuccessfully
            });
        }
        #endregion

        #region Delete a specific banking account
        /// <summary>
        /// Delete a specific banking account.
        /// </summary>
        /// <param name="getBankingAccountRequest">An object include id of banking account</param>
        /// <returns>
        /// A success message about deleting banking account.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         DELETE
        ///         
        ///         id = 1
        /// </remarks>
        /// <response code="200">Deleted a banking account Successfully.</response>
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
        [HttpDelete(APIEndPointConstant.BankingAccount.BankingAccountEndpoint)]
        public async Task<IActionResult> DeleteBankingAccountAsync([FromRoute] BankingAccountRequest getBankingAccountRequest)
        {
            ValidationResult validationResult = await this._getBankingAccountValidator.ValidateAsync(getBankingAccountRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await this._bankingAccountService.DeleteBankingAccountAsync(getBankingAccountRequest.Id, claims);
            return Ok(new
            {
                Message = MessageConstant.BankingAccountMessage.DeletedBankingAccountSuccessfully
            });
        }
        #endregion
    }
}
