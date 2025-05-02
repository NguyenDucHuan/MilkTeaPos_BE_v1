using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.ProductModel;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IProductService
    {
        Task<MethodResult<List<Product>>> CreateProductWithSizesAsync(int userId, CreateProductParentRequest parentRequest, List<ProductSizeRequest> sizes);
        Task<MethodResult<Product>> CreateExtraProductAsync(int userId, CreateExtraProductRequest request);
        Task<MethodResult<Product>> CreateComboAsync(int userId, CreateComboRequest request);
        Task<MethodResult<PaginatedResponse<ProductResponse>>> GetFilteredProductsAsync(ProductFilterModel filter);
        Task<MethodResult<ProductResponse>> GetProductByIdAsync(int id);
        Task<MethodResult<ProductResponse>> UpdateMasterProductAsync(int userId, UpdateMasterProductRequest request, List<UpdateSizeProductRequest> Variants);
        Task<MethodResult<ProductResponse>> UpdateExtraProductAsync(int userId, UpdateExtraProductRequest request);
        Task<MethodResult<ProductResponse>> UpdateComboProductAsync(int userId, UpdateComboProductRequest request);
        Task<MethodResult<bool>> UpdateProductStatusAsync(int userId, int id);
        Task<MethodResult<bool>> UpdateImageProductAsync(int productID, IFormFile? formFile);
    }
}
