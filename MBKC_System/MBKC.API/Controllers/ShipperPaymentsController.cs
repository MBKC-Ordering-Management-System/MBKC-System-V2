using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipperPaymentsController : ControllerBase
    {
        private IShipperPaymentService _shipperPaymentRepository;
        public ShipperPaymentsController(IShipperPaymentService shipperPaymentRepository)
        {
            _shipperPaymentRepository = shipperPaymentRepository;
        }
    }
}
