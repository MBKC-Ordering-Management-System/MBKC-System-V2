using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.Authorization;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.JWTs;
using MBKC.BAL.DTOs.Stores;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Services.Interfaces;
using MBKC.BAL.Utils;
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
        private IOptions<FireBaseImage> _firebaseImageOption;
        private IOptions<Email> _emailOption;
        private IValidator<CreateStoreRequest> _createStoreValidator;
        private IValidator<UpdateStoreRequest> _updateStoreValidator;
        public StoresController(IStoreService storeService, IOptions<FireBaseImage> firebaseImageOption,
            IOptions<Email> emailOption, IValidator<CreateStoreRequest> createStoreValidator, 
            IValidator<UpdateStoreRequest> updateStoreValidator)
        {
            this._storeService = storeService;
            this._firebaseImageOption = firebaseImageOption;
            this._emailOption = emailOption;
            this._createStoreValidator = createStoreValidator;
            this._updateStoreValidator = updateStoreValidator;
        }

        [PermissionAuthorize("MBKC Admin")]
        [HttpGet]
        public async Task<IActionResult> GetStoresAync([FromQuery] int? itemsPerPage, [FromQuery] int? currentPage, [FromQuery] string? searchValue)
        {
            GetStoresResponse stores = await this._storeService.GetStoresAsync(searchValue, currentPage, itemsPerPage, null);
            return Ok(stores);
        }

        [PermissionAuthorize("MBKC Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreAsync([FromRoute]int id)
        {
            GetStoreResponse store = await this._storeService.GetStoreAsync(id, null);
            return Ok(store);
        }

        [PermissionAuthorize("Brand Manager")]
        [HttpGet("/api/brand/{idBrand}/[controller]")]
        public async Task<IActionResult> GetBrandStoresAsync([FromRoute] int idBrand, [FromQuery] int? itemsPerPage, [FromQuery] int? currentPage, [FromQuery] string? searchValue)
        {
            GetStoresResponse stores = await this._storeService.GetStoresAsync(searchValue, currentPage, itemsPerPage, idBrand);
            return Ok(stores);
        }

        [PermissionAuthorize("Brand Manager")]
        [HttpGet("/api/brand/{idBrand}/[controller]/{idStore}")]
        public async Task<IActionResult> GetBrandStoreAsync([FromRoute] int idBrand, [FromRoute] int idStore)
        {
            GetStoreResponse store = await this._storeService.GetStoreAsync(idStore, idBrand);
            return Ok(store);
        }

        [PermissionAuthorize("MBKC Admin")]
        [HttpPost]
        public async Task<IActionResult> PostCreateStoreAsync([FromBody] CreateStoreRequest storeRequest)
        {
            ValidationResult validationResult = await this._createStoreValidator.ValidateAsync(storeRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._storeService.CreateStoreAsync(storeRequest, this._firebaseImageOption.Value, this._emailOption.Value);
            return Ok(new
            {
                Message = "Created New Store Successfully."
            });
        }

        [PermissionAuthorize("MBKC Admin", "Brand Manager")]
        [HttpPut("/api/brand/{idBrand}/[controller]/{idStore}")]
        public async Task<IActionResult> PutUpdateStoreAsync([FromRoute]int idBrand, [FromRoute]int idStore, [FromBody]UpdateStoreRequest updateStoreRequest)
        {
            ValidationResult validationResult = await this._updateStoreValidator.ValidateAsync(updateStoreRequest);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._storeService.UpdateStoreAsync(idBrand, idStore, updateStoreRequest, this._firebaseImageOption.Value, this._emailOption.Value);
            return Ok(new
            {
                Message = "Updated Store Successfully."
            });
        }


        [PermissionAuthorize("MBKC Admin")]
        [HttpDelete("/api/brand/{idBrand}/[controller]/{idStore}")]
        public async Task<IActionResult> DeleteStoreAsync([FromRoute] int idBrand, [FromRoute] int idStore)
        {
            await this._storeService.DeleteStoreAsync(idBrand, idStore);
            return Ok(new
            {
                Message = "Deleted Store Successfully."
            });
        }
    }
}
