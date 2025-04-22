using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Services.Interfaces;

namespace MilkTeaPosManagement.Api.Controllers
{
    [Route("api/statistic")]
    [ApiController]
    public class StatisticController(IStatisticService service) : ControllerBase
    {
        private readonly IStatisticService _service = service;
        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate(DateTime? date)
        {
            var result = await _service.OrderStatisticsByDate(date);
            return Ok(new
            {
                orderChart = result.Item1,
                revenueChart = result.Item2,
                totalOrder = result.Item3,
                totalRevenue = result.Item4
            });
        }
        [HttpGet("by-week")]
        public async Task<IActionResult> GetByWeek(DateTime? fromDate, DateTime? toDate)
        {
            var result = await _service.OrderStatisticsByWeek(fromDate, toDate);
            return Ok(new
            {
                orderChart = result.Item1,
                revenueChart = result.Item2,
                totalOrder = result.Item3,
                totalRevenue = result.Item4
            });
        }
        [HttpGet("by-month")]
        public async Task<IActionResult> GetByMonth(int? month, int? year)
        {
            var result = await _service.OrderStatisticsByMonth(month, year);
            return Ok(new
            {
                orderChart = result.Item1,
                revenueChart = result.Item2,
                totalOrder = result.Item3,
                totalRevenue = result.Item4
            });
        }
        [HttpGet("by-year")]
        public async Task<IActionResult> GetByYear(int? year)
        {
            var result = await _service.OrderStatisticsByYear(year);
            return Ok(new
            {
                orderChart = result.Item1,
                revenueChart = result.Item2,
                totalOrder = result.Item3,
                totalRevenue = result.Item4
            });
        }
        [HttpGet("best-seller")]
        public async Task<IActionResult> GetBestSellers(int number)
        {
            var result = await _service.GetBestSeller(number);
            return Ok(result);
        }
    }
}
