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
        public async Task<IActionResult> GetByDate([FromBody] DateTime? date)
        {
            var result = await _service.OrderStatisticsByDate(date);
            return Ok(result);
        }
        [HttpGet("by-week")]
        public async Task<IActionResult> GetByWeek([FromBody] DateTime? fromDate, DateTime? toDate)
        {
            var result = await _service.OrderStatisticsByWeek(fromDate, toDate);
            return Ok(result);
        }
        [HttpGet("by-month")]
        public async Task<IActionResult> GetByMonth([FromBody] int? month, int? year)
        {
            var result = await _service.OrderStatisticsByMonth(month, year);
            return Ok(result);
        }
        [HttpGet("by-year")]
        public async Task<IActionResult> GetByYear([FromBody] int? year)
        {
            var result = await _service.OrderStatisticsByYear(year);
            return Ok(result);
        }
        [HttpGet("best-seller")]
        public async Task<IActionResult> GetBestSellers([FromBody] int number)
        {
            var result = await _service.GetBestSeller(number);
            return Ok(result);
        }
    }
}
