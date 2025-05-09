using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Models.TransactionModels;
using MilkTeaPosManagement.Api.Models.VoucherMethod;
using MilkTeaPosManagement.Api.Services.Interfaces;
using System.Security.Claims;

namespace MilkTeaPosManagement.Api.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController (ITransactionService service): ControllerBase
    {
        private readonly ITransactionService _service = service;
        [HttpPut]
        public async Task<IActionResult> UpdateTransaction(int id, [FromForm] TransactionUpdateModel request)
        {
            var result = await _service.UpdateTransactionAsync(id, request);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById([FromRoute] int id)
        {
            var result = await _service.GetTransactionByIdAsync(id);

            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
    }
}
