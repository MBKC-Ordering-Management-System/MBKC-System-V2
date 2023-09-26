using MBKC.BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtraCategoriesController : ControllerBase
    {
        private IExtraCategoryService _extraCategoryService;
        public ExtraCategoriesController(IExtraCategoryService extraCategoryService)
        {
            this._extraCategoryService = extraCategoryService;
        }
    }
}
