using MBKC.Service.Services.Interfaces;
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
            this._shipperPaymentService = shipperPaymentService;
        }
    }
}
