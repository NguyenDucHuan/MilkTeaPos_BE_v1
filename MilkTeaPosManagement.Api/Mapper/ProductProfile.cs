using AutoMapper;
using MilkTeaPosManagement.Api.Models.ProductModel;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Mapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Prize))
                .ForMember(dest => dest.Sizes, opt => opt.Ignore())
                .ForMember(dest => dest.ComboItems, opt => opt.Ignore());

            CreateMap<Comboltem, ComboItemResponse>()
                .ForMember(dest => dest.ComboItemId, opt => opt.MapFrom(src => src.ComboltemId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId ?? 0))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ExtraItems, opt => opt.Ignore());
        }
    }
}
