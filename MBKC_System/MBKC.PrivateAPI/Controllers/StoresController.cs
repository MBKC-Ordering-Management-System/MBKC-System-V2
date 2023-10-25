using MBKC.PrivateAPI.Constants;
using MBKC.Service.DTOs.StorePartners;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.PrivateAPI.Controllers
{
    [ApiController]
    public class StoresController : ControllerBase
    {
        private IStoreService _storeService;
        public StoresController(IStoreService storeService)
        {
            this._storeService = storeService;
        }

        [HttpGet(APIEndPointConstant.Store.StoresEndPoint)]
        public async Task<IActionResult> GetStores()
        {
            List<GetStoreResponseForPrivateAPI> stores = await this._storeService.GetStoresAsync();
            return Ok(stores);
        }
    }
}
