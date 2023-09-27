using MBKC.BAL.Services.Interfaces;
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
    }
}
