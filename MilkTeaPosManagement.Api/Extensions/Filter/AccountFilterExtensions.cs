using MilkTeaPosManagement.Api.Models.AccountModel;
using MilkTeaPosManagement.Domain.Models;
using System.Linq.Expressions;

namespace MilkTeaPosManagement.Api.Extensions.Filter
{
    public static class AccountFilterExtensions
    {
        public static Expression<Func<Account, bool>> BuildAccountFilterExpression(this AccountFilterModel filter)
        {
            string searchTerm = filter.SearchTerm?.ToLower() ?? string.Empty;

            return user =>
                (string.IsNullOrWhiteSpace(searchTerm) ||
                 (user.Username != null && user.Username.ToLower().Contains(searchTerm)) ||
                 (user.FullName != null && user.FullName.ToLower().Contains(searchTerm)) ||
                 (user.Email != null && user.Email.ToLower().Contains(searchTerm)) ||
                 (user.Phone != null && user.Phone.Contains(searchTerm))) &&
                 
                (string.IsNullOrWhiteSpace(filter.Role) || user.Role == filter.Role) &&
                
                (!filter.Status.HasValue || user.Status == filter.Status.Value) &&
                
                (!filter.CreatedFrom.HasValue || user.CreatedAt >= filter.CreatedFrom.Value.Date) &&
                (!filter.CreatedTo.HasValue || user.CreatedAt <= filter.CreatedTo.Value.Date.AddDays(1).AddSeconds(-1));
        }
        public static IOrderedQueryable<Account> ApplySorting(this IQueryable<Account> query, string sortBy, bool ascending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return ascending
                    ? query.OrderBy(u => u.AccountId)
                    : query.OrderByDescending(u => u.AccountId);
            }

            return sortBy.ToLower() switch
            {
                "username" => ascending
                    ? query.OrderBy(u => u.Username)
                    : query.OrderByDescending(u => u.Username),

                "fullname" => ascending
                    ? query.OrderBy(u => u.FullName)
                    : query.OrderByDescending(u => u.FullName),

                "email" => ascending
                    ? query.OrderBy(u => u.Email)
                    : query.OrderByDescending(u => u.Email),

                "createdat" => ascending
                    ? query.OrderBy(u => u.CreatedAt)
                    : query.OrderByDescending(u => u.CreatedAt),

                "role" => ascending
                    ? query.OrderBy(u => u.Role)
                    : query.OrderByDescending(u => u.Role),

                "status" => ascending
                    ? query.OrderBy(u => u.Status)
                    : query.OrderByDescending(u => u.Status),

                "phone" => ascending
                    ? query.OrderBy(u => u.Phone)
                    : query.OrderByDescending(u => u.Phone),

                _ => ascending
                    ? query.OrderBy(u => u.AccountId)
                    : query.OrderByDescending(u => u.AccountId)
            };
        }
    }
}
