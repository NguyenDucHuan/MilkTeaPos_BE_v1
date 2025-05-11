using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.OrderItemModels;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Models.ProductModel;
using MilkTeaPosManagement.Api.Models.ViewModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class OrderItemService(IUnitOfWork uow, IProductService productService) : IOrderItemService
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly IProductService _productService = productService;
        public async Task<ICollection<Orderitem>> GetCartAsync()
        {
            var cart = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == null && oi.MasterId == null, include: oi => oi.Include(i => i.Product));
            var combos = await _uow.GetRepository<Product>().GetListAsync(predicate: p => p.ProductType == "Combo" && p.ParentId == null, include: p => p.Include(c => c.Comboltems));
            //var offers = GetOffers(cart, combos);
            return (cart);
        }

        //public List<Product>? GetOffers(ICollection<Orderitem> cart, ICollection<Product> combos)
        //{
        //    var offers = new List<Product>();
        //    var cartProductIds = new List<int?>();
        //    if (cart != null && combos !=null && cart.Count > 0 && combos.Count > 0)
        //    {
        //        foreach (var product in cart)
        //        {
        //            cartProductIds.Add(product.ProductId);
        //        }
        //        foreach (var combo in combos)
        //        {
        //            bool suitable = true;
        //            foreach (var comboItem in combo.Comboltems)
        //            {
        //                if (!cartProductIds.Contains(comboItem.ProductId))
        //                {
        //                    suitable = false;
        //                    break;
        //                }
        //            }
        //            if (suitable)
        //            {
        //                offers.Add(combo);
        //            }
        //        }
        //        return offers;
        //    }
        //    return null;
        //}
        public async Task<MethodResult<Orderitem>> ChangeProductsToCombo(int comboId)
        {
            var combo = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: p => p.ProductId == comboId, include: p => p.Include(c => c.Comboltems));
            foreach (var item in combo.Comboltems)
            {
                var orderItem = await _uow.GetRepository<Orderitem>().SingleOrDefaultAsync(predicate: oi => oi.ProductId == item.ProductId);
                if (orderItem.Quantity > item.Quantity)
                {
                    orderItem.Quantity -= item.Quantity;
                    _uow.GetRepository<Orderitem>().UpdateAsync(orderItem);
                } else
                {
                    _uow.GetRepository<Orderitem>().DeleteAsync(orderItem);
                }
            }
            var orderItems = await _uow.GetRepository<Orderitem>().GetListAsync();
            var itemId = orderItems.Count > 0 ? orderItems.Last().OrderItemId + 1 : 1;
            var orderItemModel = new Orderitem
            {
                OrderItemId = itemId,
                Quantity = 1,
                Price = combo.Prize,
                ProductId = combo.ProductId,
            };
            await _uow.GetRepository<Orderitem>().InsertAsync(orderItemModel);
            if(await _uow.CommitAsync() > 0)
            {
                return new MethodResult<Orderitem>.Success(orderItemModel);
            }
            return new MethodResult<Orderitem>.Failure("Combo not found!", StatusCodes.Status400BadRequest);
        }
        public async Task<ICollection<Orderitem>> GetOrderitemsByOrderIdAsync(int orderId)
        {
            return await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.MasterId == null && oi.OrderId == orderId, include: oi => oi.Include(i => i.Product));
        }
        public async Task<Orderitem> GetAnOrderItemByIdAsync(int id)
        {
            return await _uow.GetRepository<Orderitem>().SingleOrDefaultAsync(
                predicate: pm => pm.OrderItemId == id, include: oi => oi.Include(i => i.Product));
        }
        public async Task<MethodResult<Orderitem>> AddToCart(OrderItemRequest request)
        {
            try
            {
                var existeds = await _uow.GetRepository<Orderitem>().GetListAsync(
    predicate: pm => pm.ProductId == request.ProductId && pm.OrderId == null && pm.MasterId == null);
                var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: pm => pm.ProductId == request.ProductId);
                if (product == null)
                {
                    return new MethodResult<Orderitem>.Failure("Product not found!", StatusCodes.Status400BadRequest);
                }
                var toppings = new List<Product>();
                if (request.ToppingIds != null && request.ToppingIds.Count > 0)
                {
                    var index = 1;
                    foreach (var toppingId in request.ToppingIds)
                    {
                        var topping = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: tp => tp.ProductId == toppingId);
                        if (topping == null)
                        {
                            return new MethodResult<Orderitem>.Failure("Topping [#" + index + "] not found!", StatusCodes.Status400BadRequest);
                        }
                        toppings.Add(topping);
                        index++;
                    }
                }
                var comboItems = new List<Comboltem>();
                if (product.ProductType == "Combo")
                {
                    var cbIs = await _uow.GetRepository<Comboltem>().GetListAsync(predicate: ci => ci.Combod == request.ProductId);
                    foreach (var cb in cbIs)
                    {
                        comboItems.Add(cb);
                    }
                }
                var proToppings = new List<int>();
                if (product.ProductType == "SingleProduct")
                {
                    var productToppings = await _uow.GetRepository<Toppingforproduct>().GetListAsync(predicate: tp => tp.ProductId == product.ParentId);
                    foreach (var productTopping in productToppings)
                    {
                        proToppings.Add(productTopping.ToppingId);
                    }
                }
                if (existeds != null)
                {
                    foreach (var existed in existeds)
                    {
                        var existedToppings = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.MasterId == existed.OrderItemId && oi.Price > 0);
                        var existedComboItem = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.MasterId == existed.OrderItemId && oi.Price == 0);
                        var isSame = true;
                        if (existedToppings != null && request.ToppingIds != null && request.ToppingIds.Count != existedToppings.Count)
                        {
                            isSame = false;
                        }
                        else if (existedToppings != null && request.ToppingIds != null)
                        {
                            foreach (var existedTopping in existedToppings)
                            {
                                if (!request.ToppingIds.Contains(existedTopping.ProductId))
                                {
                                    isSame = false;
                                }
                            }
                        }
                        if (isSame)
                        {
                            existed.Quantity += request.Quantity;
                            existed.Price += product.Prize * request.Quantity;
                            _uow.GetRepository<Orderitem>().UpdateAsync(existed);
                            foreach (var existedTopping in existedToppings)
                            {
                                var basePrice = existedTopping.Price / existedTopping.Quantity;
                                existedTopping.Quantity += request.Quantity;
                                existedTopping.Price += basePrice * request.Quantity;
                                _uow.GetRepository<Orderitem>().UpdateAsync(existedTopping);
                            }
                            if (await _uow.CommitAsync() > 0)
                            {
                                return new MethodResult<Orderitem>.Success(existed);
                            }
                            return new MethodResult<Orderitem>.Failure("Add to cart not success", StatusCodes.Status400BadRequest);
                        }
                        if (existedComboItem != null && existedComboItem.Count > 0)
                        {
                            foreach (var existedItem in existedComboItem)
                            {
                                var baseQuntity = existedItem.Quantity / existed.Quantity;
                                existedItem.Quantity += baseQuntity;
                                _uow.GetRepository<Orderitem>().UpdateAsync(existedItem);
                            }
                            existed.Quantity += request.Quantity;
                            existed.Price += product.Prize * request.Quantity;
                            _uow.GetRepository<Orderitem>().UpdateAsync(existed);

                            if (await _uow.CommitAsync() > 0)
                            {
                                return new MethodResult<Orderitem>.Success(existed);
                            }
                            return new MethodResult<Orderitem>.Failure("Add to cart not success", StatusCodes.Status400BadRequest);
                        }
                    }
                }
                var items = await _uow.GetRepository<Orderitem>().GetListAsync();
                var itemId = items != null && items.Count > 0 ? items.Last().OrderItemId + 1 : 1;
                var item = new Orderitem
                {
                    OrderItemId = itemId,
                    Quantity = request.Quantity,
                    Price = product.Prize * request.Quantity,
                    ProductId = request.ProductId,
                };
                await _uow.GetRepository<Orderitem>().InsertAsync(item);
                foreach (var topping in toppings)
                {
                    itemId++;
                    var orderItem = new Orderitem
                    {
                        OrderItemId = itemId,
                        Quantity = request.Quantity,
                        Price = proToppings.Contains(topping.ProductId) ? 0 : topping.Prize * request.Quantity,
                        MasterId = item.OrderItemId,
                        ProductId = topping.ProductId,
                    };
                    await _uow.GetRepository<Orderitem>().InsertAsync(orderItem);
                }
                foreach (var cbi in comboItems)
                {
                    itemId++;
                    var orderItem = new Orderitem
                    {
                        OrderItemId = itemId,
                        Quantity = request.Quantity * cbi.Quantity,
                        Price = 0,
                        MasterId = item.OrderItemId,
                        ProductId = cbi.ProductId,
                    };
                    await _uow.GetRepository<Orderitem>().InsertAsync(orderItem);
                }
                if (await _uow.CommitAsync() > 0)
                {
                    return new MethodResult<Orderitem>.Success(item);
                }
                return new MethodResult<Orderitem>.Failure("Add to cart not success", StatusCodes.Status400BadRequest);

            }
            catch (Exception ex)
            {
                return new MethodResult<Orderitem>.Failure(
                    $"Error creating order item: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        public async Task<MethodResult<Orderitem>> RemoveFromCart(int orderItemId, int quantity)
        {
            try
            {
                var existed = await _uow.GetRepository<Orderitem>().SingleOrDefaultAsync(
    predicate: pm => pm.OrderItemId == orderItemId && pm.OrderId == null);
                if (existed == null)
                {
                    return new MethodResult<Orderitem>.Failure("Item not found!", StatusCodes.Status400BadRequest);
                }
                var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: pm => pm.ProductId == existed.ProductId);
                if (product == null)
                {
                    return new MethodResult<Orderitem>.Failure("Product not found!", StatusCodes.Status400BadRequest);
                }
                var toppings = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.MasterId == orderItemId && oi.Price > 0);
                var comboItems = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.MasterId == orderItemId && oi.Price == 0);

                if (existed.Quantity > quantity)
                {
                    foreach (var item in toppings)
                    {
                        var basePrice = item.Price / item.Quantity;
                        item.Quantity -= quantity;
                        item.Price -= basePrice * quantity;
                        _uow.GetRepository<Orderitem>().UpdateAsync(item);
                    }
                    foreach (var item in comboItems)
                    {
                        var baseQuantity = item.Quantity / existed.Quantity;
                        item.Quantity -= quantity * baseQuantity;
                        _uow.GetRepository<Orderitem>().UpdateAsync(item);
                    }
                    existed.Quantity -= quantity;
                    existed.Price -= product.Prize * quantity;
                    _uow.GetRepository<Orderitem>().UpdateAsync(existed);

                    if (await _uow.CommitAsync() > 0)
                    {
                        return new MethodResult<Orderitem>.Success(existed);
                    }
                }
                _uow.GetRepository<Orderitem>().DeleteAsync(existed);
                foreach (var item in toppings)
                {
                    _uow.GetRepository<Orderitem>().DeleteAsync(item);
                }
                foreach (var item in comboItems)
                {
                    _uow.GetRepository<Orderitem>().DeleteAsync(item);
                }

                if (await _uow.CommitAsync() > 0)
                {
                    return new MethodResult<Orderitem>.Success(existed);
                }
                return new MethodResult<Orderitem>.Failure("Cannot update quantity!", StatusCodes.Status400BadRequest);

            }
            catch (Exception ex)
            {
                return new MethodResult<Orderitem>.Failure(
                    $"Error remove order item: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        public async Task<MethodResult<Orderitem>> AddQuantity(int orderItemId, int quantity)
        {
            var existed = await _uow.GetRepository<Orderitem>().SingleOrDefaultAsync(
                predicate: pm => pm.OrderItemId == orderItemId && pm.OrderId == null);
            if (existed == null)
            {
                return new MethodResult<Orderitem>.Failure("Item not found!", StatusCodes.Status400BadRequest);
            }
            var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: pm => pm.ProductId == existed.ProductId);
            if (product == null)
            {
                return new MethodResult<Orderitem>.Failure("Product not found!", StatusCodes.Status400BadRequest);
            }
            var toppings = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.MasterId == existed.OrderItemId);

                existed.Quantity += quantity;
                existed.Price += product.Prize * quantity;
                _uow.GetRepository<Orderitem>().UpdateAsync(existed);

                foreach (var item in toppings)
                {
                    var basePrice = item.Price / item.Quantity;
                    item.Quantity += quantity;
                    item.Price += basePrice * quantity;
                    _uow.GetRepository<Orderitem>().UpdateAsync(item);
                }
                if (await _uow.CommitAsync() > 0)
                {
                    return new MethodResult<Orderitem>.Success(existed);
                }

            if (await _uow.CommitAsync() > 0)
            {
                return new MethodResult<Orderitem>.Success(existed);
            }
            return new MethodResult<Orderitem>.Failure("Cannot update quantity!", StatusCodes.Status400BadRequest);

        }
        public async Task<ICollection<Orderitem>> GetToppingsInCart(int masterId)
        {
            var toppings = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == null && oi.MasterId == masterId && oi.Price > 0, include: oi => oi.Include(i => i.Product));
            return toppings;
        }
        public async Task<ICollection<Orderitem>> GetComboItemsInCart(int masterId)
        {
            var toppings = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == null && oi.MasterId == masterId && oi.Price == 0, include: oi => oi.Include(i => i.Product));
            return toppings;
        }
        public async Task<ICollection<Orderitem>> GetToppingsInOrder(int orderId, int masterId)
        {
            var toppings = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == orderId && oi.MasterId == masterId, include: oi => oi.Include(i => i.Product));
            return toppings;
        }
        public async Task<MethodResult<Orderitem>> UpdateOrderItem(int id, OrderItemRequest request)
        {
            var existed = await _uow.GetRepository<Orderitem>().SingleOrDefaultAsync(
                predicate: pm => pm.OrderItemId == id && pm.OrderId == null);
            if (existed == null)
            {
                return new MethodResult<Orderitem>.Failure("Item not found!", StatusCodes.Status400BadRequest);
            }
            var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: pm => pm.ProductId == request.ProductId);
            if (product == null)
            {
                return new MethodResult<Orderitem>.Failure("Product not found!", StatusCodes.Status400BadRequest);
            }
            var existedToppings = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.MasterId == id);
            var existedComboItem = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.MasterId == id && oi.Price == 0);
            foreach (var existedTopping in existedToppings)
            {
                _uow.GetRepository<Orderitem>().DeleteAsync(existedTopping);
            }
            foreach (var comboItem in existedComboItem)
            {
                _uow.GetRepository<Orderitem>().DeleteAsync(comboItem);
            }
            if (await _uow.CommitAsync() < 0)
            {
                return new MethodResult<Orderitem>.Failure("Edit order item fail!", StatusCodes.Status400BadRequest);
            }
            var items = await _uow.GetRepository<Orderitem>().GetListAsync();
            var itemId = items != null && items.Count > 0 ? items.Last().OrderItemId + 1 : 1;
            if (product.ProductType == "Combo")
            {
                var cbIs = await _uow.GetRepository<Comboltem>().GetListAsync(predicate: ci => ci.Combod == request.ProductId);                
                foreach (var cb in cbIs)
                {
                    var orderItem = new Orderitem
                    {
                        OrderItemId = itemId,
                        Quantity = request.Quantity,
                        Price = 0,
                        MasterId = existed.OrderItemId,
                        ProductId = cb.ProductId,
                    };
                    itemId++;
                    await _uow.GetRepository<Orderitem>().InsertAsync(orderItem);
                }
            }
            if (request.ToppingIds != null && request.ToppingIds.Count > 0)
            {
                var index = 1;
                foreach (var toppingId in request.ToppingIds)
                {
                    var topping = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: tp => tp.ProductId == toppingId);
                    if (topping == null)
                    {
                        return new MethodResult<Orderitem>.Failure("Topping [#" + index + "] not found!", StatusCodes.Status400BadRequest);
                    }
                    var orderItem = new Orderitem
                    {
                        OrderItemId = itemId,
                        Quantity = request.Quantity,
                        Price = topping.Prize * request.Quantity,
                        MasterId = existed.OrderItemId,
                        ProductId = topping.ProductId,
                    };
                    itemId++;
                    await _uow.GetRepository<Orderitem>().InsertAsync(orderItem);
                    index++;
                }
            }
            existed.Quantity = request.Quantity;
            existed.Price = request.Quantity * product.Prize;
            existed.ProductId = request.ProductId;
            _uow.GetRepository<Orderitem>().UpdateAsync(existed);
            if (await _uow.CommitAsync() <= 0)
            {
                return new MethodResult<Orderitem>.Failure("Edit order item fail!", StatusCodes.Status400BadRequest);
            }
            return new MethodResult<Orderitem>.Success(existed);
        }
        public async Task<ProductResponse> GetProductByIdAsync(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            return result.Match(
                (errorMessage, statusCode) => new ProductResponse(),
                product => product
            );
        }
    }
}
