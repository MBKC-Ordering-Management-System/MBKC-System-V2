using MBKC.Service.DTOs.BankingAccounts;
using MBKC.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankingAccountsController : ControllerBase
    {
        private IBankingAccountService _bankingAccountService;
        public BankingAccountsController(IBankingAccountService bankingAccountService)
        {
            this._bankingAccountService = bankingAccountService;
        }

        [HttpGet("/api/kitchencenter/{idKitchenCenter}/[controller]")]
        public async Task<IActionResult> GetBankingAccountsAsync([FromRoute] int idKitchenCenter)
        {
            return Ok();
        }

        [HttpGet("/api/kitchencenter/{idKitchenCenter}/[controller]/{idBankingAccount}")]
        public async Task<IActionResult> GetBankingAccountAsync([FromRoute] int idKitchenCenter, [FromRoute] int idBankingAccount)
        {
            return Ok();
        }

        [HttpPost("/api/kitchencenter/{idKitchenCenter}/[controller]")]
        public async Task<IActionResult> PostCreateBankingAccountAsync([FromRoute] int idKitchenCenter, [FromForm]CreateBankingAccountRequest bankingAccountRequest)
        {
            return Ok();
        }

        [HttpPut("/api/kitchencenter/{idKitchenCenter}/[controller]/{idBankingAccount}")]
        public async Task<IActionResult> PutUpdateBankingAccountAsync([FromRoute] int idKItchenCenter, [FromRoute] int idBankingAccount, [FromForm]UpdateBankingAccountRequest bankingAccountRequest)
        {
            return Ok();
        }

        [HttpPut("/api/kitchencenter/{idKitchenCenter}/[controller]/{idBankingAccount}/updating-status")]
        public async Task<IActionResult> PutUpdateBankingAccountStatusAsync([FromRoute] int idKItchenCenter, [FromRoute] int idBankingAccount, [FromBody] UpdateBankingAccountStatusRequest bankingAccountStatusRequest)
        {
            return Ok();
        }

        [HttpDelete("/api/kitchencenter/{idKitchenCenter}/[controller]/{idBankingAccount}")]
        public async Task<IActionResult> DeleteBankingAccountAsync([FromRoute] int idKitchenCenter, [FromRoute] int idBankingAccount)
        {
            return Ok();
        }
    }
}
