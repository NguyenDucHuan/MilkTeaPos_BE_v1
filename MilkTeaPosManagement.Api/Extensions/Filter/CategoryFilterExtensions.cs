using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Extensions.Filter
{
    public static class CategoryFilterExtensions
    {
        public static IOrderedQueryable<Category> ApplySorting(this IQueryable<Category> query, string sortBy, bool ascending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return ascending ? query.OrderBy(c => c.CategoryId) : query.OrderByDescending(c => c.CategoryId);
            }
            switch (sortBy.ToLower())
            {
                case "categoryname":
                    return ascending ? query.OrderBy(c => c.CategoryName) : query.OrderByDescending(c => c.CategoryName);
                case "productcount":
                    return ascending
                        ? query.OrderBy(c => c.Products.Count)
                        : query.OrderByDescending(c => c.Products.Count);
                default:
                    return ascending ? query.OrderBy(c => c.CategoryId) : query.OrderByDescending(c => c.CategoryId);
            }
        }
    }
}
