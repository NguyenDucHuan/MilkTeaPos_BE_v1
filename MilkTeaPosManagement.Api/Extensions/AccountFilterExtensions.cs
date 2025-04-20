using MilkTeaPosManagement.Api.Models.AccountModel;
using MilkTeaPosManagement.Domain.Models;
using System.Linq.Expressions;

namespace MilkTeaPosManagement.Api.Extensions
{
    public static class AccountFilterExtensions
    {
        public static Expression<Func<Account, bool>> BuildFilterExpression(this AccountFilterModel filter)
        {
            // Start with a base predicate that always returns true
            Expression<Func<Account, bool>> predicate = user => true;

            // Build filter based on search term
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                string searchTerm = filter.SearchTerm.ToLower();
                return user =>
                    (user.Username != null && user.Username.ToLower().Contains(searchTerm)) ||
                    (user.FullName != null && user.FullName.ToLower().Contains(searchTerm)) ||
                    (user.Email != null && user.Email.ToLower().Contains(searchTerm)) ||
                    (user.Phone != null && user.Phone.Contains(searchTerm));
            }

            // Filter by role
            if (!string.IsNullOrWhiteSpace(filter.Role))
            {
                predicate = user => user.Role == filter.Role;
            }

            // Filter by status
            if (filter.Status.HasValue)
            {
                predicate = user => user.Status == filter.Status.Value;
            }

            // Filter by creation date range
            if (filter.CreatedFrom.HasValue)
            {
                DateTime startDate = filter.CreatedFrom.Value.Date;
                predicate = user => user.CreatedAt >= startDate;
            }

            if (filter.CreatedTo.HasValue)
            {
                DateTime endDate = filter.CreatedTo.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                predicate = user => user.CreatedAt <= endDate;
            }

            return predicate;
        }

        public static IOrderedQueryable<Account> ApplySorting(
            this IQueryable<Account> query,
            string sortBy,
            bool ascending)
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
