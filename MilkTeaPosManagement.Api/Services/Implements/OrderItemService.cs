using Microsoft.EntityFrameworkCore;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.OrderItemModels;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class OrderItemService(IUnitOfWork uow) : IOrderItemService
    {
        private readonly IUnitOfWork _uow = uow;
        public async Task<(ICollection<Orderitem>, List<Product>?)> GetCartAsync()
        {
            var cart = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == null, include: oi => oi.Include(i => i.Product));
            var combos = await _uow.GetRepository<Product>().GetListAsync(predicate: p => p.ProductType == "Combo" && p.ParentId == null, include: p => p.Include(c => c.Orderitems));
            var offers = GetOffers(cart, combos);
            return (cart, offers);
        }

        public List<Product> GetOffers(ICollection<Orderitem> cart, ICollection<Product> combos)
        {
            var offers = new List<Product>();
            var cartProductIds = new List<int?>();
            foreach (var product in cart)
            {
                cartProductIds.Add(product.ProductId);
            }
            foreach (var combo in combos)
            {
                bool suitable = true;
                foreach (var comboItem in combo.Comboltems)
                {
                    if (!cartProductIds.Contains(comboItem.ProductId))
                    {
                        suitable = false;
                        break;
                    }
                }
                if (suitable)
                {
                    offers.Add(combo);
                }
            }
            return offers;
        }
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
            return await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId != null && oi.OrderId == orderId, include: oi => oi.Include(i => i.Product));
        }
        public async Task<Orderitem> GetAnOrderItemByIdAsync(int id)
        {
            return await _uow.GetRepository<Orderitem>().SingleOrDefaultAsync(
                predicate: pm => pm.OrderItemId == id, include: oi => oi.Include(i => i.Product));
        }
        public async Task<MethodResult<Orderitem>> AddToCart(OrderItemRequest request)
        {
            var existed = await _uow.GetRepository<Orderitem>().SingleOrDefaultAsync(
                predicate: pm => pm.ProductId == request.ProductId && pm.OrderId == null);
            var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: pm => pm.ProductId == request.ProductId);
            if (product == null)
            {
                return new MethodResult<Orderitem>.Failure("Product not found!", StatusCodes.Status400BadRequest);
            }
            if (existed != null)
            {
                existed.Quantity += request.Quantity;             
                existed.Price += product.Prize * request.Quantity;
                _uow.GetRepository<Orderitem>().UpdateAsync(existed);
                if (await _uow.CommitAsync() > 0)
                {
                    return new MethodResult<Orderitem>.Success(existed);
                }                
            }
            var items = await _uow.GetRepository<Orderitem>().GetListAsync();
            var itemId = items != null && items.Count > 0 ? items.Last().OrderItemId + 1 : 1;
            var item = new Orderitem
            {
                OrderItemId = itemId,
                Quantity = request.Quantity,
                Price = product.Prize,
                MasterId = request.MasterId,
                ProductId = request.ProductId,
            };
            await _uow.GetRepository<Orderitem>().InsertAsync(item);
            if (await _uow.CommitAsync() > 0)
            {
                return new MethodResult<Orderitem>.Success(item);
            }
            return new MethodResult<Orderitem>.Failure("Add to cart not success", StatusCodes.Status400BadRequest);
            
        }
        public async Task<MethodResult<Orderitem>> RemoveFromCart(int productId, int quantity)
        {
            var existed = await _uow.GetRepository<Orderitem>().SingleOrDefaultAsync(
                predicate: pm => pm.ProductId == productId && pm.OrderId == null);
            var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: pm => pm.ProductId == productId);
            if (product == null)
            {
                return new MethodResult<Orderitem>.Failure("Product not found!", StatusCodes.Status400BadRequest);
            }
            if (existed == null)
            {
                return new MethodResult<Orderitem>.Failure("Item not found!", StatusCodes.Status400BadRequest);
            }
            if (existed.Quantity > quantity)
            {
                existed.Quantity-= quantity;
                existed.Price -= product.Prize * quantity;
                _uow.GetRepository<Orderitem>().UpdateAsync(existed);
                if (await _uow.CommitAsync() > 0)
                {
                    return new MethodResult<Orderitem>.Success(existed);
                }
            }
            _uow.GetRepository<Orderitem>().DeleteAsync(existed);
            if (await _uow.CommitAsync() > 0)
            {
                return new MethodResult<Orderitem>.Success(existed);
            }
            return new MethodResult<Orderitem>.Failure("Remove from cart not success!", StatusCodes.Status400BadRequest);
            
        }
    }
}
