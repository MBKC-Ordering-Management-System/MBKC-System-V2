using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private IPartnerService _partnerRepository;
        public PartnersController(IPartnerService partnerRepository)
        {
            _partnerRepository = partnerRepository;
        }
    }
}
