using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.Authorization;
using MBKC.BAL.DTOs.Accounts;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.KitchenCenters;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Errors;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Services.Interfaces;
using MBKC.BAL.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Web;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenCentersController : ControllerBase
    {
        private IKitchenCenterService _kitchenCenterService;
        private IOptions<FireBaseImage> _firebaseImageOption;
        private IOptions<Email> _emailOption;
        private IValidator<CreateKitchenCenterRequest> _createKitchenCenterValidator;
        private IValidator<UpdateKitchenCenterRequest> _updateKitchenCenterValidator;
        public KitchenCentersController(IKitchenCenterService kitchenCenterService, IOptions<FireBaseImage> firebaseImageOption,
            IOptions<Email> emailOption, IValidator<CreateKitchenCenterRequest> createKitchenCenterValidator, 
            IValidator<UpdateKitchenCenterRequest> updateKitchenCenterValidator)
        {
            this._kitchenCenterService = kitchenCenterService;
            this._firebaseImageOption = firebaseImageOption;
            this._emailOption = emailOption;
            this._createKitchenCenterValidator = createKitchenCenterValidator;
            this._updateKitchenCenterValidator = updateKitchenCenterValidator;
        }

        #region Get KitchenCenters
        /// <summary>
        /// Get all kitchen centers in the system.
        /// </summary>
        /// <param name="itemsPerPage">Number of kitchen centers on a page.</param>
        /// <param name="currentPage">The current page number.</param>
        /// <param name="keySearchName">The search value by kitchen center name.</param>
        /// <param name="isGetAll">Input TRUE if you want to get all kitchen centers, ignoring currentPage and itemsPerPage, otherwise Input FALSE.</param>
        /// <returns>
        /// An Object contains TotalPage, NumberItems and a list of kitchen centers with some conditions (itemsPerPage, currentPage, searchValue) (if have).
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET 
        ///         itemsPerPage = 5
        ///         currentPage = 1
        ///         searchValue = Bình Thạnh
        /// </remarks>
        /// <response code="200">Get a list of kitchen centers Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetKitchenCentersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [PermissionAuthorize("MBKC Admin")]
        [HttpGet]
        public async Task<IActionResult> GetKitchenCentersAsync([FromQuery]int? itemsPerPage, [FromQuery]int? currentPage, [FromQuery]string? keySearchName, [FromQuery] bool? isGetAll)
        {
            GetKitchenCentersResponse getKitchenCentersResponse = await this._kitchenCenterService.GetKitchenCentersAsync(itemsPerPage, currentPage, keySearchName, isGetAll);
            return Ok(getKitchenCentersResponse);
        }
        #endregion

        #region Get KitchenCenter
        /// <summary>
        /// Get a specific kitchen center by kitchen center id in the system.
        /// </summary>
        /// <param name="id">The kitchen center's id</param>
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
        [Produces("application/json")]
        [PermissionAuthorize("MBKC Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKitchenCenterAsync([FromRoute]int id)
        {
            GetKitchenCenterResponse getKitchenCenterResponse = await this._kitchenCenterService.GetKitchenCenterAsync(id);
            return Ok(getKitchenCenterResponse);
        }
        #endregion

        #region Create New KitchenCenter
        /// <summary>
        /// Create new kitchen center.
        /// </summary>
        /// <param name="kitchenCenter">A kitchen center object contains created information.</param>
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
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [PermissionAuthorize("MBKC Admin")]
        [HttpPost]
        public async Task<IActionResult> PostCreateKitchenCenterAsync([FromForm]CreateKitchenCenterRequest kitchenCenter)
        {
            ValidationResult validationResult = await this._createKitchenCenterValidator.ValidateAsync(kitchenCenter);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._kitchenCenterService.CreateKitchenCenterAsync(kitchenCenter, this._emailOption.Value, this._firebaseImageOption.Value);
            return Ok(new
            {
                Message = "Created Kitchen Center Successfully."
            });
        }
        #endregion

        #region Update Existed KitchenCenter
        /// <summary>
        /// Update information of an existed kitchen center.
        /// </summary>
        /// <param name="id">The kitchen center's id.</param>
        /// <param name="kitchenCenter">An kitchen center object contains updated information.</param>
        /// <returns>
        /// A success message about updating kitchen center information.  
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT 
        ///         id = 1
        ///         
        ///         {
        ///             "Name": "Kitchen Center Example"
        ///             "Address": "Đường expamle, Tỉnh example"
        ///             "Status": "Active | Inactive"
        ///             "NewLogo": Imgage File
        ///             "DeletedLogo": "Image URL"
        ///             "ManagerEmail": "abc@example.com"
        ///         }
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
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [PermissionAuthorize("MBKC Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUpdateKitchenCenterAsync([FromRoute]int id, [FromForm]UpdateKitchenCenterRequest kitchenCenter)
        {
            ValidationResult validationResult = await this._updateKitchenCenterValidator.ValidateAsync(kitchenCenter);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._kitchenCenterService.UpdateKitchenCenterAsync(id, kitchenCenter, this._emailOption.Value, this._firebaseImageOption.Value);
            return Ok(new
            {
                Message = "Updated Kitchen Center Information Successfully."
            });
        }
        #endregion

        #region Delete Existed KitchenCenter
        /// <summary>
        /// Delete an existed kitchen center.
        /// </summary>
        /// <param name="id">The kitchen center's id</param>
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
        [Produces("application/json")]
        [PermissionAuthorize("MBKC Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKitchenCenterAsync([FromRoute]int id)
        {
            await this._kitchenCenterService.DeleteKitchenCenterAsync(id);
            return Ok(new
            {
                Message = "Deleted Kitchen Center Successfully"
            });
        }
        #endregion
    }
}
