using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipperPaymentsController : ControllerBase
    {
        private IShipperPaymentService _shipperPaymentService;
        public ShipperPaymentsController(IShipperPaymentService shipperPaymentService)
        {
            _shipperPaymentService = shipperPaymentService;
        }
    }
}
