using MBKC.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashiersController : ControllerBase
    {
        private ICashierService _cashierService;
        public CashiersController(ICashierService cashierService)
        {
            this._cashierService = cashierService;
        }
    }
}
