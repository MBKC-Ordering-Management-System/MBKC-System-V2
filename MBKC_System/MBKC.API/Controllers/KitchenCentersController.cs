using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenCentersController : ControllerBase
    {
        private IKitchenCenterService _kitchenCenterService;
        public KitchenCentersController(IKitchenCenterService kitchenCenterService)
        {
            _kitchenCenterService = kitchenCenterService;
        }
    }
}
