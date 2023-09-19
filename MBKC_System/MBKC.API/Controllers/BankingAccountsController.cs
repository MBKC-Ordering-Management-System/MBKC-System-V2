using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankingAccountsController : ControllerBase
    {
        private IBankingAccountRepository _bankingAccountRepository;
        public BankingAccountsController(IBankingAccountRepository bankingAccountRepository)
        {
            _bankingAccountRepository = bankingAccountRepository;
        }
    }
}
