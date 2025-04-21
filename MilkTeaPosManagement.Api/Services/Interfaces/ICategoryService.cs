using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.CategoryModels;
using MilkTeaPosManagement.Api.Models.ViewModels;
using MilkTeaPosManagement.Domain.Paginate;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IPaginate<CategoryViewModel>> GetCategoriesByFilterAsync(CategoryFilterModel filter);
        Task<MethodResult<CategoryViewModel>> GetCategoryByIdAsync(int id);
        Task<MethodResult<CategoryViewModel>> CreateCategoryAsync(CreateCategoryRequest request);
        Task<MethodResult<CategoryViewModel>> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
        Task<MethodResult<bool>> DeleteCategoryAsync(int id);
    }
}
