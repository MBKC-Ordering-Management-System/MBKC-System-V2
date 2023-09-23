using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.KitchenCenters;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Repositories.Interfaces;
using MBKC.BAL.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Web;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenCentersController : ControllerBase
    {
        private IKitchenCenterRepository _kitchenCenterRepository;
        private IOptions<FireBaseImage> _firebaseImageOption;
        private IOptions<Email> _emailOption;
        private IValidator<CreateKitchenCenterRequest> _createKitchenCenterValidator;
        private IValidator<UpdateKitchenCenterRequest> _updateKitchenCenterValidator;
        public KitchenCentersController(IKitchenCenterRepository kitchenCenterRepository, IOptions<FireBaseImage> firebaseImageOption,
            IOptions<Email> emailOption, IValidator<CreateKitchenCenterRequest> createKitchenCenterValidator, 
            IValidator<UpdateKitchenCenterRequest> updateKitchenCenterValidator)
        {
            this._kitchenCenterRepository = kitchenCenterRepository;
            this._firebaseImageOption = firebaseImageOption;
            this._emailOption = emailOption;
            this._createKitchenCenterValidator = createKitchenCenterValidator;
            this._updateKitchenCenterValidator = updateKitchenCenterValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetKitchenCentersAsync([FromQuery]int? itemsPerPage, [FromQuery]int? currentPage, [FromQuery]string? searchValue)
        {
            GetKitchenCentersResponse getKitchenCentersResponse = await this._kitchenCenterRepository.GetKitchenCentersAsync(itemsPerPage, currentPage, searchValue);
            return Ok(getKitchenCentersResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKitchenCenterAsync([FromRoute]int id)
        {
            GetKitchenCenterResponse getKitchenCenterResponse = await this._kitchenCenterRepository.GetKitchenCenterAsync(id);
            return Ok(getKitchenCenterResponse);
        }

        [HttpPost]
        public async Task<IActionResult> PostCreateKitchenCenterAsync([FromForm]CreateKitchenCenterRequest kitchenCenter)
        {
            ValidationResult validationResult = await this._createKitchenCenterValidator.ValidateAsync(kitchenCenter);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._kitchenCenterRepository.CreateKitchenCenterAsync(kitchenCenter, this._emailOption.Value, this._firebaseImageOption.Value);
            return Ok(new
            {
                Message = "Created Kitchen Center Successfully."
            });
        }

        /*[HttpPut("{id}")]
        public async Task<IActionResult> PutUpdateKitchenCenterAsync([FromRoute]int id, [FromForm]UpdateKitchenCenterRequest kitchenCenter)
        {
            ValidationResult validationResult = await this._updateKitchenCenterValidator.ValidateAsync(kitchenCenter);
            if(validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
        }*/
    }
}
