using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorePartnersController : ControllerBase
    {
        private IStorePartnerRepository _storePartnerRepository;
        public StorePartnersController(IStorePartnerRepository storePartnerRepository)
        {
            _storePartnerRepository = storePartnerRepository;
        }
    }
}
