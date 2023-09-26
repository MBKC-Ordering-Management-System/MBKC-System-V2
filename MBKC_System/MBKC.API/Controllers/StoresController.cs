using MBKC.BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private IStoreService _storeService;
        public StoresController(IStoreService storeService)
        {
            this._storeService = storeService;
        }
    }
}
