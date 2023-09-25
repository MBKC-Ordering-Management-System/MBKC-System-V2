using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorePartnersController : ControllerBase
    {
        private IStorePartnerService _storePartnerService;
        public StorePartnersController(IStorePartnerService storePartnerService)
        {
            _storePartnerService = storePartnerService;
        }
    }
}
