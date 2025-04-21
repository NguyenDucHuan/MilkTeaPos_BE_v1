using AutoMapper;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.CategoryModels;
using MilkTeaPosManagement.Api.Models.ViewModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Domain.Paginate;
using System.Linq.Expressions;
using MilkTeaPosManagement.Api.Extensions.Filter;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IPaginate<CategoryViewModel>> GetCategoriesByFilterAsync(CategoryFilterModel filter)
        {
            if (filter == null)
                filter = new CategoryFilterModel();

            string searchTerm = filter.SearchTerm?.ToLower() ?? string.Empty;

            Expression<Func<Category, bool>> predicate = c =>
                (string.IsNullOrWhiteSpace(searchTerm) ||
                 (c.CategoryName != null && c.CategoryName.ToLower().Contains(searchTerm)) ||
                 (c.Description != null && c.Description.ToLower().Contains(searchTerm))) &&
                (!filter.Status.HasValue || c.Status == filter.Status.Value);

            return await _uow.GetRepository<Category>().GetPagingListAsync(
                selector: category => _mapper.Map<CategoryViewModel>(category),
                predicate: predicate,
                orderBy: query => query.ApplySorting(filter.SortBy, filter.SortAscending),
                page: filter.Page,
                size: filter.PageSize
            );
        }
        public async Task<MethodResult<CategoryViewModel>> GetCategoryByIdAsync(int id)
        {
            var category = await _uow.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: c => c.CategoryId == id
            );

            if (category == null)
            {
                return new MethodResult<CategoryViewModel>.Failure("Category not found", StatusCodes.Status404NotFound);
            }

            var categoryViewModel = _mapper.Map<CategoryViewModel>(category);
            return new MethodResult<CategoryViewModel>.Success(categoryViewModel);
        }

        public async Task<MethodResult<CategoryViewModel>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            try
            {
                // Check if category with same name already exists
                var existingCategory = await _uow.GetRepository<Category>().SingleOrDefaultAsync(
                    predicate: c => c.CategoryName == request.CategoryName
                );

                if (existingCategory != null)
                {
                    return new MethodResult<CategoryViewModel>.Failure(
                        "Category with this name already exists",
                        StatusCodes.Status400BadRequest
                    );
                }

                // Create new category
                var category = new Category
                {
                    CategoryName = request.CategoryName,
                    Description = request.Description,
                    Status = request.Status
                };

                await _uow.GetRepository<Category>().InsertAsync(category);

                if (await _uow.CommitAsync() > 0)
                {
                    var categoryViewModel = _mapper.Map<CategoryViewModel>(category);
                    return new MethodResult<CategoryViewModel>.Success(categoryViewModel);
                }

                return new MethodResult<CategoryViewModel>.Failure(
                    "Failed to create category",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<CategoryViewModel>.Failure(
                    $"Error creating category: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<MethodResult<CategoryViewModel>> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
        {
            try
            {
                var category = await _uow.GetRepository<Category>().SingleOrDefaultAsync(
                    predicate: c => c.CategoryId == id
                );

                if (category == null)
                {
                    return new MethodResult<CategoryViewModel>.Failure(
                        "Category not found",
                        StatusCodes.Status404NotFound
                    );
                }
                if (!string.IsNullOrEmpty(request.CategoryName) && request.CategoryName != category.CategoryName)
                {
                    var nameExists = await _uow.GetRepository<Category>().SingleOrDefaultAsync(
                        predicate: c => c.CategoryName == request.CategoryName && c.CategoryId != id
                    );

                    if (nameExists != null)
                    {
                        return new MethodResult<CategoryViewModel>.Failure(
                            "Category name already in use",
                            StatusCodes.Status400BadRequest
                        );
                    }
                }

                if (!string.IsNullOrEmpty(request.CategoryName))
                    category.CategoryName = request.CategoryName;

                if (request.Description != null)
                    category.Description = request.Description;

                if (request.Status.HasValue)
                    category.Status = request.Status.Value;

                _uow.GetRepository<Category>().UpdateAsync(category);

                if (await _uow.CommitAsync() > 0)
                {
                    var categoryViewModel = _mapper.Map<CategoryViewModel>(category);
                    return new MethodResult<CategoryViewModel>.Success(categoryViewModel);
                }

                return new MethodResult<CategoryViewModel>.Failure(
                    "Failed to update category",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<CategoryViewModel>.Failure(
                    $"Error updating category: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<MethodResult<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _uow.GetRepository<Category>().SingleOrDefaultAsync(
                    predicate: c => c.CategoryId == id
                );

                if (category == null)
                {
                    return new MethodResult<bool>.Failure(
                        "Category not found",
                        StatusCodes.Status404NotFound
                    );
                }

                if (category.Products.Any())
                {
                    return new MethodResult<bool>.Failure(
                        "Cannot delete category with associated products",
                        StatusCodes.Status400BadRequest
                    );
                }

                _uow.GetRepository<Category>().DeleteAsync(category);

                if (await _uow.CommitAsync() > 0)
                {
                    return new MethodResult<bool>.Success(true);
                }

                return new MethodResult<bool>.Failure(
                    "Failed to delete category",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<bool>.Failure(
                    $"Error deleting category: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
