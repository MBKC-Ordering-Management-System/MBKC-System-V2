using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Accounts;
using MBKC.Service.DTOs.KitchenCenters;
using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class KitchenCentersController : ControllerBase
    {
        private IKitchenCenterService _kitchenCenterService;
        private IValidator<CreateKitchenCenterRequest> _createKitchenCenterValidator;
        private IValidator<UpdateKitchenCenterRequest> _updateKitchenCenterValidator;
        private IValidator<UpdateKitchenCenterStatusRequest> _updateKitchenCenterStatusValidator;
        private IValidator<GetKitchenCentersRequest> _getKitchenCentersValidator;
        private IValidator<KitchenCenterIdRequest> _getKitchenCenterValidator;

        public KitchenCentersController(IKitchenCenterService kitchenCenterService,
            IValidator<CreateKitchenCenterRequest> createKitchenCenterValidator,
            IValidator<GetKitchenCentersRequest> getKitchenCentersValidator,
            IValidator<UpdateKitchenCenterRequest> updateKitchenCenterValidator,
            IValidator<KitchenCenterIdRequest> getKitchenCenterValidator,
            IValidator<UpdateKitchenCenterStatusRequest> updateKitchenCenterStatusValidator)
        {
            this._kitchenCenterService = kitchenCenterService;
            this._createKitchenCenterValidator = createKitchenCenterValidator;
            this._updateKitchenCenterValidator = updateKitchenCenterValidator;
            this._updateKitchenCenterStatusValidator = updateKitchenCenterStatusValidator;
            this._getKitchenCentersValidator = getKitchenCentersValidator;
            this._getKitchenCenterValidator = getKitchenCenterValidator;
        }

        #region Get KitchenCenters
        /// <summary>
        /// Get all kitchen centers in the system.
        /// </summary>
        /// <param name="getKitchenCentersRequest">An object include SearchValue, ItemsPerPage, CurrentPage, SortBy, IsGetAll for search, sort, paging.</param>
        /// <returns>
        /// An Object contains TotalPage, NumberItems and a list of kitchen centers with some conditions (itemsPerPage, currentPage, searchValue, sortBy, isGetAll) (if have).
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         searchValue = Center Quận 9
        ///         currentPage = 1
        ///         itemsPerPage = 5
        ///         sortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        ///         isGetAll = True | False
        /// </remarks>
        /// <response code="200">Get a list of kitchen centers Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetKitchenCentersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin, PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.KitchenCenter.KitchenCentersEndpoint)]
        public async Task<IActionResult> GetKitchenCentersAsync([FromQuery] GetKitchenCentersRequest getKitchenCentersRequest)
        {
            ValidationResult validationResult = await this._getKitchenCentersValidator.ValidateAsync(getKitchenCentersRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            GetKitchenCentersResponse getKitchenCentersResponse = await this._kitchenCenterService.GetKitchenCentersAsync(getKitchenCentersRequest);
            return Ok(getKitchenCentersResponse);
        }
        #endregion

        #region Get KitchenCenter
        /// <summary>
        /// Get a specific kitchen center by kitchen center id in the system.
        /// </summary>
        /// <param name="kitchenCenterId">An object include id of kitchen center</param>
        /// <returns>
        /// An object about a specific kitchen center
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         id = 1
        /// </remarks>
        /// <response code="200">Get a specific kitchen center by id Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpGet(APIEndPointConstant.KitchenCenter.KitchenCenterEndpoint)]
        public async Task<IActionResult> GetKitchenCenterAsync([FromRoute] KitchenCenterIdRequest kitchenCenterId)
        {
            ValidationResult validationResult = await this._getKitchenCenterValidator.ValidateAsync(kitchenCenterId);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            GetKitchenCenterResponse getKitchenCenterResponse = await this._kitchenCenterService.GetKitchenCenterAsync(kitchenCenterId.Id);
            return Ok(getKitchenCenterResponse);
        }
        #endregion

        #region Get Kitchen Center Profile
        /// <summary>
        /// Get kitchen center profile.
        /// </summary>
        /// <returns>
        /// An object about a specific kitchen center
        /// </returns>
        /// <response code="200">Get a specific kitchen center by id Successfully.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetKitchenCenterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager)]
        [HttpGet(APIEndPointConstant.KitchenCenter.KitchenCenterProfileEndpoint)]
        public async Task<IActionResult> GetKitchenCenterProfileAsync()
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            GetKitchenCenterResponse getKitchenCenterResponse = await this._kitchenCenterService.GetKitchenCenterProfileAsync(claims);
            return Ok(getKitchenCenterResponse);
        }
        #endregion

        #region Create New KitchenCenter
        /// <summary>
        /// Create new kitchen center.
        /// </summary>
        /// <param name="newKitchenCenter">A kitchen center object contains created information.</param>
        /// <returns>
        /// A success message about creating kitchen center information.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "Name": "Kitchen Center Example"
        ///             "Address": "Đường expamle, Tỉnh example"
        ///             "Logo": [Imgage File]
        ///             "ManagerEmail": "abc@example.com"
        ///         }
        /// </remarks>
        /// <response code="200">Created new kitchen center successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpPost(APIEndPointConstant.KitchenCenter.KitchenCentersEndpoint)]
        public async Task<IActionResult> PostCreateKitchenCenterAsync([FromForm] CreateKitchenCenterRequest newKitchenCenter)
        {
            ValidationResult validationResult = await this._createKitchenCenterValidator.ValidateAsync(newKitchenCenter);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._kitchenCenterService.CreateKitchenCenterAsync(newKitchenCenter);
            return Ok(new
            {
                Message = MessageConstant.KitchenCenterMessage.CreatedNewKitchenCenterSuccessfully
            });
        }
        #endregion

        #region Update Existed KitchenCenter
        /// <summary>
        /// Update information of an existed kitchen center.
        /// </summary>
        /// <param name="updatedKitchenCenter">An object include id of kitchen center.</param>
        /// <param name="kitchenCenterId">An kitchen center object contains updated information.</param>
        /// <returns> 
        /// A success message about updating kitchen center information.  
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         id = 1
        ///         Name = Kitchen Center Example
        ///         Address = Đường expamle, Tỉnh example
        ///         Status = Active | Inactive
        ///         Logo = [Imgage File]
        ///         ManagerEmail = abc@example.com
        /// </remarks>
        /// <response code="200">Updated kitchen center successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpPut(APIEndPointConstant.KitchenCenter.KitchenCenterEndpoint)]
        public async Task<IActionResult> PutUpdateKitchenCenterAsync([FromRoute] KitchenCenterIdRequest kitchenCenterId, [FromForm] UpdateKitchenCenterRequest updatedKitchenCenter)
        {


            ValidationResult validationResultKCId = await this._getKitchenCenterValidator.ValidateAsync(kitchenCenterId);
            ValidationResult validationResult = await this._updateKitchenCenterValidator.ValidateAsync(updatedKitchenCenter);
            if (validationResultKCId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultKCId);
                throw new BadRequestException(errors);
            }
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._kitchenCenterService.UpdateKitchenCenterAsync(kitchenCenterId.Id, updatedKitchenCenter);
            return Ok(new
            {
                Message = MessageConstant.KitchenCenterMessage.UpdatedKitchenCenterSuccessfully
            });
        }
        #endregion

        #region Update Kitchen Center Status
        /// <summary>
        /// Update status of an existed kitchen center.
        /// </summary>
        /// <param name="kitchenCenterId">An object include id of kitchen center.</param>
        /// <param name="updatedKitchenCenterStatus">An kitchen center object contains updated status.</param>
        /// <returns>
        /// A success message about updating kitchen center's status.  
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         id = 1
        ///         {
        ///             "Status": "Active | Inactive"
        ///         }
        /// </remarks>
        /// <response code="200">Updated kitchen center status successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpPut(APIEndPointConstant.KitchenCenter.UpdatingStatusKitchenCenter)]
        public async Task<IActionResult> PutUpdateKitchenCenterStatusAsync([FromRoute] KitchenCenterIdRequest kitchenCenterId, [FromBody] UpdateKitchenCenterStatusRequest updatedKitchenCenterStatus)
        {
            ValidationResult validationResult = await this._updateKitchenCenterStatusValidator.ValidateAsync(updatedKitchenCenterStatus);
            ValidationResult validationResultKCId = await this._getKitchenCenterValidator.ValidateAsync(kitchenCenterId);

            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            if (validationResultKCId.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResultKCId);
                throw new BadRequestException(errors);
            }
            await this._kitchenCenterService.UpdateKitchenCenterStatusAsync(kitchenCenterId.Id, updatedKitchenCenterStatus);
            return Ok(new
            {
                Message = MessageConstant.KitchenCenterMessage.UpdatedKitchenCenterStatusSuccessfully
            });
        }
        #endregion

        #region Delete Existed KitchenCenter
        /// <summary>
        /// Delete an existed kitchen center.
        /// </summary>
        /// <param name="kitchenCenterId">An object include id of kitchen center.</param>
        /// <returns>
        /// A success message about deleting exsited kitchen center.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         DELETE 
        ///         id = 1
        /// </remarks>
        /// <response code="200">Deleted kitchen center successfully.</response>
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
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpDelete(APIEndPointConstant.KitchenCenter.KitchenCenterEndpoint)]
        public async Task<IActionResult> DeleteKitchenCenterAsync([FromRoute] KitchenCenterIdRequest kitchenCenterId)
        {
            ValidationResult validationResult = await this._getKitchenCenterValidator.ValidateAsync(kitchenCenterId);

            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._kitchenCenterService.DeleteKitchenCenterAsync(kitchenCenterId.Id);
            return Ok(new
            {
                Message = MessageConstant.KitchenCenterMessage.DeletedKitchenCenterSuccessfully
            });
        }
        #endregion
    }
}
