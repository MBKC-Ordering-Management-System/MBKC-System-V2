using FluentValidation;
using FluentValidation.Results;
using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.ShipperPayments;
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
    public class ShipperPaymentsController : ControllerBase
    {
        private IShipperPaymentService _shipperPaymentService;
        private IValidator<GetShipperPaymentsRequest> _getShipperPaymentsValidator;
        public ShipperPaymentsController(IShipperPaymentService shipperPaymentService, 
            IValidator<GetShipperPaymentsRequest> getShipperPaymentsValidator)
        {
            this._shipperPaymentService = shipperPaymentService;
            this._getShipperPaymentsValidator = getShipperPaymentsValidator;
        }
        #region Get shipper payments by cashier id, kitchencenter id, store id.
        /// <summary>
        ///  Get shipper payments by cashier id, kitchencenter id or store id.
        /// </summary>
        /// <param name="getShipperPaymentsRequest">
        /// An object include  ItemsPerPage, CurrentPage, SortBy, 
        /// Status, SearchDateFrom, SearchDateTo, PaymentMethod for search, sort, filter
        /// </param>
        /// <returns>
        /// List Shipper payments.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET
        ///         CurrentPage = 1
        ///         ItemsPerPage = 5
        ///         SearchDateFrom = 27/07/2023
        ///         SearchDateTo = 20/10/2023
        ///         PaymentMethod = CASH
        ///         Status = SUCCESS
        ///         SortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
        /// </remarks>
        /// <response code="200">Get shipper payments successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetShipperPaymentsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier, PermissionAuthorizeConstant.KitchenCenterManager, PermissionAuthorizeConstant.StoreManager)]
        [HttpGet(APIEndPointConstant.ShipperPayment.ShipperPaymentsEndpoint)]
        public async Task<IActionResult> GetShipperPaymentsAsync([FromQuery] GetShipperPaymentsRequest getShipperPaymentsRequest)
        {
            ValidationResult validationResult = await this._getShipperPaymentsValidator.ValidateAsync(getShipperPaymentsRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getMoneyExchangesResponse = await this._shipperPaymentService.GetShipperPayments(getShipperPaymentsRequest, claims);
            return Ok(getMoneyExchangesResponse);
        }
        #endregion
    }
}
