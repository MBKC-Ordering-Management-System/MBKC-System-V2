using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private IWalletService _walletRepository;
        public WalletsController(IWalletService walletRepository)
        {
            _walletRepository = walletRepository;
        }
    }
}
