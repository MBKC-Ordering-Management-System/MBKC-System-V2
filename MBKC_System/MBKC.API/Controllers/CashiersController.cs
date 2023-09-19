using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashiersController : ControllerBase
    {
        private ICashierRepository _cashierRepository;
        public CashiersController(ICashierRepository cashierRepository)
        {
            _cashierRepository = cashierRepository;
        }
    }
}
