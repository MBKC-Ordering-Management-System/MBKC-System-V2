using FluentValidation;
using MBKC.BAL.Authorization;
using MBKC.BAL.DTOs.JWTs;
using MBKC.BAL.DTOs.Stores;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private IStoreService _storeService;
        private IOptions<JWTAuth> _jwtAuthOption;
        private IOptions<Email> _emailOption;
        private IValidator<CreateStoreRequest> _createStoreValidator;
        private IValidator<UpdateStoreRequest> _updateStoreValidator;
        public StoresController(IStoreService storeService, IOptions<JWTAuth> jwtAuthOption,
            IOptions<Email> emailOption, IValidator<CreateStoreRequest> createStoreValidator, 
            IValidator<UpdateStoreRequest> updateStoreValidator)
        {
            this._storeService = storeService;
            this._jwtAuthOption = jwtAuthOption;
            this._emailOption = emailOption;
            this._createStoreValidator = createStoreValidator;
            this._updateStoreValidator = updateStoreValidator;
        }

        [PermissionAuthorize("MBKC Admin")]
        [HttpGet]
        public async Task<IActionResult> GetStoresAync([FromQuery] int? itemsPerPage, [FromQuery] int? currentPage, [FromQuery] string? searchValue)
        {
            return Ok();
        }

        [PermissionAuthorize("MBKC Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreAsync()
        {
            return Ok();
        }

        [PermissionAuthorize("Brand Manager")]
        [HttpGet("/api/brand/{idBrand}/[controller]")]
        public async Task<IActionResult> GetBrandStoresAsync([FromRoute] int idBrand, [FromQuery] int? itemsPerPage, [FromQuery] int? currentPage, [FromQuery] string? searchValue)
        {
            return Ok();
        }

        [PermissionAuthorize("Brand Manager")]
        [HttpGet("/api/brand/{idBrand}/[controller]/{idStore}")]
        public async Task<IActionResult> GetBrandStoreAsync([FromRoute] int idBrand, [FromRoute] int idStore)
        {
            return Ok();
        }

        [PermissionAuthorize("MBKC Admin")]
        [HttpPost]
        public async Task<IActionResult> PostCreateStoreAsync([FromBody] CreateStoreRequest storeRequest)
        {
            return Ok();
        }

        [PermissionAuthorize("MBKC Admin", "Brand Manager")]
        [HttpPut("/api/brand/{idBrand}/[controller]/{idStore}")]
        public async Task<IActionResult> PutUpdateStoreAsync([FromRoute]int idBrand, [FromRoute]int idStore, [FromBody]UpdateStoreRequest updateStoreRequest)
        {
            return Ok();
        }


        [PermissionAuthorize("MBKC Admin")]
        [HttpDelete("/api/brand/{idBrand}/[controller]/{idStore}")]
        public async Task<IActionResult> DeleteStoreAsync([FromRoute] int idBrand, [FromRoute] int idStore)
        {
            return Ok();
        }
    }
}
