using MBKC.API.Constants;
using MBKC.Service.Authorization;
using MBKC.Service.DTOs.Products;
using MBKC.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MBKC.API.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductService _productService;
        public ProductsController(IProductService productService)
        {
            this._productService = productService;
        }


        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager, PermissionAuthorizeConstant.Store_Manager)]
        [HttpGet(APIEndPointConstant.Product.ProductsEndpoint)]
        public async Task<IActionResult> GetProductsAsync([FromQuery]string? searchName, [FromQuery] int? currentPage, 
            [FromQuery] int? itemsPerPage, [FromQuery] string? productType, [FromQuery] bool? isGetAll, [FromQuery] int? idCategory)
        {
            return Ok();
        }

        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager, PermissionAuthorizeConstant.Store_Manager)]
        [HttpGet(APIEndPointConstant.Product.ProductEndpoint)]
        public async Task<IActionResult> GetProductAsync([FromRoute] int id)
        {
            return Ok();
        }

        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpPost(APIEndPointConstant.Product.ProductsEndpoint)]
        public async Task<IActionResult> PostCreatNewProduct([FromForm]CreateProductRequest createProductRequest)
        {
            return Ok();
        }

        [Consumes(MediaTypeConstant.Multipart_Form_Data)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpPut(APIEndPointConstant.Product.ProductEndpoint)]
        public async Task<IActionResult> PutUpdateProductAsync([FromRoute] int id, [FromForm] UpdateProductRequest updateProductRequest)
        {
            return Ok();
        }

        [Consumes(MediaTypeConstant.Application_Json)]
        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpPut(APIEndPointConstant.Product.UpdatingStatusProductEndpoint)]
        public async Task<IActionResult> PutUpdateProductStatusAsync([FromRoute] int id, [FromBody] UpdateProductStatusRequest updateProductStatusRequest)
        {
            return Ok();
        }

        [Produces(MediaTypeConstant.Application_Json)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Brand_Manager)]
        [HttpDelete(APIEndPointConstant.Product.ProductEndpoint)]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] int id)
        {
            return Ok();
        }
    }
}
