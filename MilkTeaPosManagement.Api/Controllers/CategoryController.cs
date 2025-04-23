using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Models.CategoryModels;
using MilkTeaPosManagement.Api.Routes;
using MilkTeaPosManagement.Api.Services.Interfaces;

namespace MilkTeaPosManagement.Api.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route(Router.CategoryRoute.GetAll)]
        public async Task<IActionResult> GetCategories([FromQuery] CategoryFilterModel filter = null)
        {
            if (filter == null)
            {
                filter = new CategoryFilterModel();
            }

            var categories = await _categoryService.GetCategoriesByFilterAsync(filter);
            return Ok(categories);
        }

        [HttpGet]
        [Route(Router.CategoryRoute.GetById)]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }

        //[Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPost]
        [Route(Router.CategoryRoute.Create)]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryRequest request)
        {
            var result = await _categoryService.CreateCategoryAsync(request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }

        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpPut]
        [Route(Router.CategoryRoute.Update)]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] UpdateCategoryRequest request)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }

        [Authorize(Roles = UserConstant.USER_ROLE_MANAGER)]
        [HttpDelete]
        [Route(Router.CategoryRoute.Delete)]
        public async Task<IActionResult> UpdateStatusCategory(int id)
        {
            var result = await _categoryService.UpdateStatus(id);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                _ => Ok(new { message = "Category Update Status successfully" })
            );
        }
    }
}
