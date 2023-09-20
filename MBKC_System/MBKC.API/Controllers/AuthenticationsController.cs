using FluentValidation;
using FluentValidation.Results;
using MBKC.BAL.DTOs.Accounts;
using MBKC.BAL.DTOs.AccountTokens;
using MBKC.BAL.DTOs.JWTs;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Repositories.Interfaces;
using MBKC.BAL.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class AuthenticationsController : ControllerBase
    {
        private IAuthenticationRepository _authenticationRepository;
        private IOptions<JWTAuth> _jwtAuthOptions;
        private IValidator<AccountRequest> _accountRequestValidator;
        private IValidator<AccountTokenRequest> _accountTokenRequestValidator;
        public AuthenticationsController(IAuthenticationRepository authenticationRepository, IOptions<JWTAuth> jwtAuthOptions,
            IValidator<AccountRequest> accountRequestValidator, IValidator<AccountTokenRequest> accountTokenRequestValidator)
        {
            this._authenticationRepository = authenticationRepository;
            this._jwtAuthOptions = jwtAuthOptions;
            this._accountRequestValidator = accountRequestValidator;
            this._accountTokenRequestValidator = accountTokenRequestValidator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> PostLoginAsync([FromBody]AccountRequest account)
        {
            ValidationResult validationResult = await this._accountRequestValidator.ValidateAsync(account);
            if(validationResult.IsValid == false)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }

            AccountResponse accountResponse = await this._authenticationRepository.LoginAsync(account, this._jwtAuthOptions.Value);
            return Ok(accountResponse);
        }

        [HttpPost("tokens-regeneration")]
        public async Task<IActionResult> PostReGenerateTokensAsync([FromBody]AccountTokenRequest accountToken)
        {
            ValidationResult validationResult = await this._accountTokenRequestValidator.ValidateAsync(accountToken);
            if(validationResult.IsValid == false)
            {
                string error = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(error);
            }
            AccountTokenResponse accountTokenResponse = await this._authenticationRepository.ReGenerateTokensAsync(accountToken, this._jwtAuthOptions.Value);
            return Ok(accountTokenResponse);
        }
    }
}
