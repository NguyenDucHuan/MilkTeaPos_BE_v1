using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IStatisticService
    {
        Task<(List<object>, List<object>, int, decimal?)> OrderStatisticsByDate(DateTime? date);
        Task<(List<object>, List<object>, int, decimal?)> OrderStatisticsByWeek(DateTime? fromDate, DateTime? toDate);
        Task<(List<object>, List<object>, int, decimal?)> OrderStatisticsByMonth(int? month, int? year);
        Task<(List<object>, List<object>, int, decimal?)> OrderStatisticsByYear(int? year);
        Task<List<object>> GetBestSeller(int number);
    }
}
