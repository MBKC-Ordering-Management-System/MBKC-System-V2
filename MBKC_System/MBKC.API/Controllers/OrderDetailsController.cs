using MBKC.BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private IOrderDetailService _orderDetailService;
        public OrderDetailsController(IOrderDetailService orderDetailService)
        {
            this._orderDetailService = orderDetailService;
        }
    }
}
