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
    }
}
