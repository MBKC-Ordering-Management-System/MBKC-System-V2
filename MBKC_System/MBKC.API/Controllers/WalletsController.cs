using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private IWalletService _walletService;
        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }
    }
}
