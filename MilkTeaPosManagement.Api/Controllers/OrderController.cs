using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Models.OrderModels;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Domain.Paginate;

namespace MilkTeaPosManagement.Api.Controllers
{
    [Route("api/order")]
    [ApiController]
    //[Authorize]
    public class OrderController(IOrderService service, IOrderItemService orderItemService) : ControllerBase
    {
        private readonly IOrderService _service = service;
        private readonly IOrderItemService _orderItemService = orderItemService;
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] OrderSearchModel? searchModel)
        {
            var result = await _service.GetAllOrders(searchModel);
            if (result.Item1 == 0)
            {
                return BadRequest(result.Item3);
            }
            var orderList = new List<object>();
            foreach (var item in result.Item2.Items)
            {
                orderList.Add(new
                {
                    orderId = item.OrderId,
                    createAt = item.CreateAt,
                    totalAmount = item.TotalAmount,
                    note = item.Note,
                    staffId = item.StaffId,
                    staffName = item.Staff.FullName,
                    paymentMethodId = item.PaymentMethodId,
                    paymentMethodName = item.PaymentMethod.MethodName,
                    orderStatus = item.Orderstatusupdates.FirstOrDefault().OrderStatus
                });
            }
            return Ok(new
            {
                size = result.Item2.Size,
                page = result.Item2.Page,
                total = result.Item2.Total,
                totalPages = result.Item2.TotalPages,
                items = orderList
            });
            //return Ok(result.Item2);
        }
        [HttpGet("get-by-id/{orderId}")]
        public async Task<IActionResult> Get([FromRoute]int orderId)
        {
            var result = await _service.GetOrderDetail(orderId);
            if (result.Item1 == 400)
            {
                return BadRequest(result.Item3);
            }
            //var mainProducts = result.Item2.Orderitems.Where(oi => oi.MasterId == null).ToList();

            //var productList = new List<object>();
            //foreach(var product in mainProducts)
            //{
            //    var toppings = await _orderItemService.GetToppingsInOrder(result.Item2.OrderId, product.OrderItemId);
            //    var toppingOfProduct = new List<object>();
            //    decimal? toppingPrice = 0;
            //    foreach (var topping in toppings)
            //    {
            //        toppingOfProduct.Add(new
            //        {
            //            toppingId = topping.ProductId,
            //            toppingName = topping.Product.ProductName,
            //            toppingPrize = topping.Product.Prize,
            //        });
            //        toppingPrice += topping.Product.Prize;
            //    }
            //    productList.Add(new
            //    {
            //        orderItemId = product.OrderItemId,
            //        quantity = product.Quantity,
            //        subPrice = product.Product.Prize + toppingPrice,
            //        productId = product.ProductId,
            //        productName = product.Product.ProductName,
            //        productImg = product.Product.ImageUrl,
            //        productPrice = product.Product.Prize,
            //        size = product.Product.SizeId,
            //        toppings = toppingOfProduct
            //    });
            //}

            //var order = new
            //{
            //    orderId = result.Item2.OrderId,
            //    createAt = result.Item2.CreateAt,
            //    totalAmount = result.Item2.TotalAmount,
            //    note = result.Item2.Note,
            //    staffId = result.Item2.StaffId,
            //    staffName = result.Item2?.Staff?.FullName,
            //    paymentMethodId = result.Item2?.PaymentMethodId,
            //    paymentMethodName = result.Item2?.PaymentMethod?.MethodName,
            //    orderStatus = result.Item2?.Orderstatusupdates?.FirstOrDefault()?.OrderStatus,
            //    products = productList
            //};
            //return Ok(order);
            return Ok(result.Item2);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] OrderRequest request)
        {
            var result = await _service.CreateOrder(request);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [HttpDelete("cancel-order")]
        public async Task<IActionResult> Delete([FromBody] int orderId)
        {
            var result = await _service.CancelOrder(orderId);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
        [HttpPut("confirm-order")]
        public async Task<IActionResult> Update([FromBody] int orderId)
        {
            var result = await _service.ConfirmOrder(orderId);
            return result.Match(
                (errorMessage, statusCode) => Problem(detail: errorMessage, statusCode: statusCode),
                Ok
            );
        }
    }
}
