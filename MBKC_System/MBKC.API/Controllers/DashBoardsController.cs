using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.DashBoards;
using MBKC.Service.DTOs.DashBoards.Brand;
using MBKC.Service.DTOs.DashBoards.Cashier;
using MBKC.Service.DTOs.DashBoards.KitchenCenter;
using MBKC.Service.Errors;
using MBKC.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class DashBoardsController : ControllerBase
    {
        private IDashBoardService _dashBoardService;
        public DashBoardsController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        #region Get admin dash board
        /// <summary>
        ///  Get admin dash board.
        /// <summary>
        /// </summary>
        /// <returns>
        /// An object include information admin dash board.
        /// </returns>
        /// <response code="200">Get dash board successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetAdminDashBoardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.MBKCAdmin)]
        [HttpGet(APIEndPointConstant.DashBoard.AdminDashBoardEndpoint)]
        public async Task<IActionResult> GetAdminDashBoardAsync()
        {
            var getAdminDashBoard = await this._dashBoardService.GetAdminDashBoardAsync();
            return Ok(getAdminDashBoard);
        }
        #endregion

        #region Get kitchen center dash board
        /// <summary>
        ///  Get kitchen center dash board.
        /// <summary>
        /// </summary>
        /// <returns>
        /// An object include information kitchen center dash board.
        /// </returns>
        /// <response code="200">Get dash board successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetKitchenCenterDashBoardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.KitchenCenterManager)]
        [HttpGet(APIEndPointConstant.DashBoard.KitchenCenterDashBoardEndpoint)]
        public async Task<IActionResult> GetKitchenCenterDashBoardAsync()
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getKitchenCenterDashBoard = await this._dashBoardService.GetKitchenCenterDashBoardAsync(claims);
            return Ok(getKitchenCenterDashBoard);
        }
        #endregion

        #region Get brand dash board
        /// <summary>
        ///  Get brand dash board.
        /// <summary>
        /// </summary>
        /// <returns>
        /// An object include information brand dash board.
        /// </returns>
        /// <response code="200">Get dash board successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetBrandDashBoardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.BrandManager)]
        [HttpGet(APIEndPointConstant.DashBoard.BrandDashBoardEndpoint)]
        public async Task<IActionResult> GetBrandDashBoardAsync([FromQuery] GetSearchDateDashBoardRequest getSearchDateDashBoardRequest)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getBrandDashBoard = await this._dashBoardService.GetBrandDashBoardAsync(claims, getSearchDateDashBoardRequest);
            return Ok(getBrandDashBoard);
        }
        #endregion

        #region Get store dash board
        /// <summary>
        ///  Get store dash board.
        /// <summary>
        /// </summary>
        /// <returns>
        /// An object include information store dash board.
        /// </returns>
        /// <response code="200">Get dash board successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetStoreDashBoardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.StoreManager)]
        [HttpGet(APIEndPointConstant.DashBoard.StoreDashBoardEndpoint)]
        public async Task<IActionResult> GetStoreDashBoardAsync()
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getStoreDashBoard = await this._dashBoardService.GetStoreDashBoardAsync(claims);
            return Ok(getStoreDashBoard);
        }
        #endregion

        #region Get cashier dash board
        /// <summary>
        ///  Get cashier dash board.
        /// <summary>
        /// </summary>
        /// <returns>
        /// An object include information cashier dash board.
        /// </returns>
        /// <response code="200">Get dash board successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(GetCashierDashBoardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Cashier)]
        [HttpGet(APIEndPointConstant.DashBoard.CashierDashBoardEndpoint)]
        public async Task<IActionResult> GetCashierDashBoardAsync([FromQuery] GetSearchDateDashBoardRequest getSearchDateDashBoardRequest)
        {
            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            var getBrandDashBoard = await this._dashBoardService.GetCashierDashBoardAsync(claims);
            return Ok(getBrandDashBoard);
        }
        #endregion
    }
}
