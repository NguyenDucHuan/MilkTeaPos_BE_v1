using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Models.ProductModel;
using MilkTeaPosManagement.Api.Routes;
using MilkTeaPosManagement.Api.Services.Interfaces;
using System.Security.Claims;

namespace MilkTeaPosManagement.Api.Controllers
{
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPost]
        [Route(Router.ProductRoute.Create_Mater_Producr)]
        public async Task<IActionResult> CreateParentProduct([FromForm] CreateProductParentRequest parentRequest)
        {
            var userIdString = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID.");
            }

            if (parentRequest.Sizes == null || !parentRequest.Sizes.Any())
            {
                return BadRequest("At least one size is required");
            }

            var result = await _productService.CreateProductWithSizesAsync(userId, parentRequest, parentRequest.Sizes);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                products => Ok(new
                {
                    message = "Products created successfully",
                    data = products
                })
            );
        }
        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPost]
        [Route(Router.ProductRoute.Create_Extra_Product)]
        public async Task<IActionResult> CreateExtraProduct([FromForm] CreateExtraProductRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var result = await _productService.CreateExtraProductAsync(userId, request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                product => Ok(new
                {
                    message = "Extra product created successfully",
                    data = product
                })
            );
        }
        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPost]
        [Route(Router.ProductRoute.Create_Combo)]
        public async Task<IActionResult> CreateCombo([FromForm] CreateComboRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var result = await _productService.CreateComboAsync(userId, request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                combo => Ok(new
                {
                    message = "Combo created successfully",
                    data = combo
                })
            );
        }
        [Authorize]
        [HttpGet]
        [Route(Router.ProductRoute.GetAll)]
        public async Task<IActionResult> GetFilteredProducts([FromQuery] ProductFilterModel filter)
        {
            var result = await _productService.GetFilteredProductsAsync(filter);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                products => Ok(new
                {
                    message = "Products retrieved successfully",
                    data = products
                })
            );
        }
    }
}
