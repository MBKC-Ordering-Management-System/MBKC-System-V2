using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Wallets;
using MBKC.Service.Errors;
using MBKC.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private IWalletService _walletService;
        public WalletsController(IWalletService walletService)
        {
            this._walletService = walletService;
        }

        #region Get wallet by cashier id, kitchencenter id, store id
        /// <summary>
        ///  Get wallet by cashier id, kitchencenter id or store id
        /// <summary>
        /// </summary>
        /// <returns>
        /// An object include information about money exchange, wallet, transaction, shipper payment.
        /// </returns>
         /// <response code="200">Get wallet successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetWalletResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier, PermissionAuthorizeConstant.KitchenCenterManager, PermissionAuthorizeConstant.StoreManager)]
        [HttpGet(APIEndPointConstant.Wallet.WalletEndpoint)]
        public async Task<IActionResult> GetWalletInformationAsync([FromQuery] GetSearchDateWalletRequest searchDateWalletRequest)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getOrderResponse = await this._walletService.GetWallet(searchDateWalletRequest, claims);
            return Ok(getOrderResponse);
        }
        #endregion
    }
}
