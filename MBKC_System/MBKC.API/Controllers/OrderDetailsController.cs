using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private IOrderDetailService _orderDetailRepository;
        public OrderDetailsController(IOrderDetailService orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }
    }
}
