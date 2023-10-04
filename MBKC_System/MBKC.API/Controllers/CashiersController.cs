using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class CashiersController : ControllerBase
    {
        private ICashierService _cashierService;
        public CashiersController(ICashierService cashierService)
        {
            this._cashierService = cashierService;
        }

        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Kitchen_Center_Manager)]
        [HttpGet(APIEndPointConstant.Cashier.CashiersEndpoint)]
        public async Task<IActionResult> GetCashiersAsync([FromQuery]string? searchValue, [FromQuery]int? itemsPerPage, 
            [FromQuery]int? currentPage)
        {
            return Ok();
        }

        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Kitchen_Center_Manager)]
        [HttpGet(APIEndPointConstant.Cashier.CashierEndpoint)]
        public async Task<IActionResult> GetCashierAsync([FromRoute]int id)
        {
            return Ok();
        }

        /*public async Task<IActionResult> PostCreateCashierAsync()
        {

        }*/
    }
}
