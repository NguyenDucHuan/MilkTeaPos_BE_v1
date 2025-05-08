using AutoMapper;
using MilkTeaPosManagement.Api.Models.OrderModels;
using MilkTeaPosManagement.Api.Models.ProductModel;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Mapper
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderResponse>();
        }
    }
}
