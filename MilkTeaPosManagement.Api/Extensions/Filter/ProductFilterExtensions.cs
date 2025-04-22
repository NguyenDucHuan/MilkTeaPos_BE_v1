using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Models.ProductModel;
using MilkTeaPosManagement.Domain.Models;
using System.Linq.Expressions;

namespace MilkTeaPosManagement.Api.Extensions.Filter
{
    public static class ProductFilterExtensions
    {
        public static Expression<Func<Product, bool>> BuildProductFilterExpression(this ProductFilterModel filter)
        {
            string searchTerm = filter.SearchTerm?.ToLower() ?? string.Empty;

            return product =>
                (string.IsNullOrWhiteSpace(searchTerm) ||
                 (product.ProductName != null && product.ProductName.ToLower().Contains(searchTerm)) ||
                 (product.Description != null && product.Description.ToLower().Contains(searchTerm))) &&
                (!filter.CategoryId.HasValue || product.CategoryId == filter.CategoryId) &&
                (string.IsNullOrWhiteSpace(filter.ProductType) || product.ProductType == filter.ProductType) &&
                (!filter.MinPrice.HasValue || product.Prize >= filter.MinPrice) &&
                (!filter.MaxPrice.HasValue || product.Prize <= filter.MaxPrice) &&
                (!filter.Status.HasValue || product.Status == filter.Status) &&
                (filter.IsShopManager ||
                 product.ProductType == ProductConstant.PRODUCT_TYPE_MATTER_PRODUCT ||
                 product.ProductType == ProductConstant.PRODUCT_TYPE_COMBO ||
                 product.ProductType == ProductConstant.PRODUCT_TYPE_EXTRA_PRODUCT);
        }

        public static IOrderedQueryable<Product> ApplyProductSorting(this IQueryable<Product> query, string sortBy, bool ascending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return ascending
                    ? query.OrderBy(p => p.ProductId)
                    : query.OrderByDescending(p => p.ProductId);
            }

            return sortBy.ToLower() switch
            {
                "productid" => ascending
                    ? query.OrderBy(p => p.ProductId)
                    : query.OrderByDescending(p => p.ProductId),

                "productname" => ascending
                    ? query.OrderBy(p => p.ProductName)
                    : query.OrderByDescending(p => p.ProductName),

                "categoryname" => ascending
                    ? query.OrderBy(p => p.Category.CategoryName)
                    : query.OrderByDescending(p => p.Category.CategoryName),

                "price" => ascending
                    ? query.OrderBy(p => p.Prize)
                    : query.OrderByDescending(p => p.Prize),

                "createdat" => ascending
                    ? query.OrderBy(p => p.CreateAt)
                    : query.OrderByDescending(p => p.CreateAt),

                "status" => ascending
                    ? query.OrderBy(p => p.Status)
                    : query.OrderByDescending(p => p.Status),

                _ => ascending
                    ? query.OrderBy(p => p.ProductId)
                    : query.OrderByDescending(p => p.ProductId)
            };
        }
    }
}
