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
        [Authorize]
        [HttpGet]
        [Route(Router.ProductRoute.GetById)]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                product => Ok(new
                {
                    message = "Product retrieved successfully",
                    data = product
                })
            );
        }
        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPut]
        [Route(Router.ProductRoute.UpdateMaster)]
        public async Task<IActionResult> UpdateMasterProduct([FromForm] UpdateMasterProductRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var result = await _productService.UpdateMasterProductAsync(userId, request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                product => Ok(new
                {
                    message = "Master product updated successfully",
                    data = product
                })
            );
        }

        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPut]
        [Route(Router.ProductRoute.UpdateSize)]
        public async Task<IActionResult> UpdateSizeProduct([FromForm] UpdateSizeProductRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var result = await _productService.UpdateSizeProductAsync(userId, request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                product => Ok(new
                {
                    message = "Size product updated successfully",
                    data = product
                })
            );
        }

        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPut]
        [Route(Router.ProductRoute.UpdateExtra)]
        public async Task<IActionResult> UpdateExtraProduct([FromForm] UpdateExtraProductRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var result = await _productService.UpdateExtraProductAsync(userId, request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                product => Ok(new
                {
                    message = "Extra product updated successfully",
                    data = product
                })
            );
        }

        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPut]
        [Route(Router.ProductRoute.UpdateCombo)]
        public async Task<IActionResult> UpdateComboProduct([FromForm] UpdateComboProductRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var result = await _productService.UpdateComboProductAsync(userId, request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                product => Ok(new
                {
                    message = "Combo product updated successfully",
                    data = product
                })
            );
        }
        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPut]
        [Route(Router.ProductRoute.Delete)]
        public async Task<IActionResult> UpdateProductStatus(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID.");
            }
            var result = await _productService.UpdateProductStatusAsync(userId, id);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                product => Ok(new
                {
                    message = "Product status updated successfully",
                    data = product
                })
            );
        }
    }
}
