using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtraCategoriesController : ControllerBase
    {
        private IExtraCategoryRepository _extraCategoryRepository;
        public ExtraCategoriesController(IExtraCategoryRepository extraCategoryRepository)
        {
            _extraCategoryRepository = extraCategoryRepository;
        }
    }
}
