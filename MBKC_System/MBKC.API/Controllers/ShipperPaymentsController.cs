using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class ShipperPaymentsController : ControllerBase
    {
        public ShipperPaymentsController()
        {
            /*#region Get shipper payments by cashier id, kitchencenter id.
            /// <summary>
            ///  Get shipper payments by cashier id, kitchencenter id.
            /// </summary>
            /// <param name="getMoneyExchangesRequest">
            /// An object include  ItemsPerPage, CurrentPage, SortBy, 
            /// Status, SearchDateFrom, SearchDateTo, ExchangeType for search, sort, filter
            /// </param>
            /// <returns>
            /// An object include information about money exchange.
            /// </returns>
            /// <remarks>
            ///     Sample request:
            ///     
            ///         GET
            ///         CurrentPage = 1
            ///         ItemsPerPage = 5
            ///         SearchDateFrom = 27/07/2023
            ///         SearchDateTo = 20/10/2023
            ///         ExchangeType = SEND
            ///         Status = SUCCESS
            ///         SortBy = "propertyName_asc | propertyName_ASC | propertyName_desc | propertyName_DESC"
            /// </remarks>
            /// <response code="200">Get money exchange successfully.</response>
            /// <response code="400">Some Error about request data and logic data.</response>
            /// <response code="404">Some Error about request data not found.</response>
            /// <response code="500">Some Error about the system.</response>
            /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
            /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
            /// <exception cref="Exception">Throw Error about the system.</exception>
            [ProducesResponseType(typeof(GetMoneyExchangesResponse), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
            [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
            [Consumes(MediaTypeConstant.ApplicationJson)]
            [Produces(MediaTypeConstant.ApplicationJson)]
            [PermissionAuthorize(PermissionAuthorizeConstant.Cashier, PermissionAuthorizeConstant.KitchenCenterManager, PermissionAuthorizeConstant.StoreManager)]
            [HttpGet(APIEndPointConstant.MoneyExchange.MoneyExchangesEndpoint)]
            public async Task<IActionResult> GetMoneyExchangesAsync([FromQuery] GetMoneyExchangesRequest getMoneyExchangesRequest)
            {
                ValidationResult validationResult = await this._getMoneyExchangesValidator.ValidateAsync(getMoneyExchangesRequest);
                if (validationResult.IsValid == false)
                {
                    string errors = ErrorUtil.GetErrorsString(validationResult);
                    throw new BadRequestException(errors);
                }
                IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
                var getMoneyExchangesResponse = await this._moneyExchangeService.GetMoneyExchanges(claims, getMoneyExchangesRequest);
                return Ok(getMoneyExchangesResponse);
            }
            #endregion*/
        }
    }
}
