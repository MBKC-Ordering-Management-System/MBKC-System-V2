﻿using MBKC.BAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtraCategoriesController : ControllerBase
    {
        private IExtraCategoryService _extraCategoryRepository;
        public ExtraCategoriesController(IExtraCategoryService extraCategoryRepository)
        {
            _extraCategoryRepository = extraCategoryRepository;
        }
    }
}
