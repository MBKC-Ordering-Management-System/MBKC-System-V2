using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipperPaymentsController : ControllerBase
    {
        private IShipperPaymentRepository _shipperPaymentRepository;
        public ShipperPaymentsController(IShipperPaymentRepository shipperPaymentRepository)
        {
            _shipperPaymentRepository = shipperPaymentRepository;
        }
    }
}
