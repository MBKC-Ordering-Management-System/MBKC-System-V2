using MBKC.PrivateAPI.Constants;
using MBKC.Service.DTOs.Configurations;
using MBKC.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.PrivateAPI.Controllers
{
    [ApiController]
    public class ConfigurationsController : ControllerBase
    {
        private IConfigurationService _configurationService;
        public ConfigurationsController(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
        }


        [HttpGet(APIEndPointConstant.Configuration.ConfigurationsEndPoint)]
        public async Task<IActionResult> GetConfigurationsAsync()
        {
            List<GetConfigurationResponse> getConfigurationResponses = await this._configurationService.GetConfigurationsAsync();
            return Ok(getConfigurationResponses);
        }
    }
}
