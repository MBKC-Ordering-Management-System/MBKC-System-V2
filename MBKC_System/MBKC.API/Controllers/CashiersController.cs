using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashiersController : ControllerBase
    {
        private ICashierService _cashierRepository;
        public CashiersController(ICashierService cashierRepository)
        {
            _cashierRepository = cashierRepository;
        }
    }
}
