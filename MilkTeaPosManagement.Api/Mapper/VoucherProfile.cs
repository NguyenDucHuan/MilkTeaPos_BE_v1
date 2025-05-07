using AutoMapper;
using MilkTeaPosManagement.Api.Models.VoucherMethod;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Mapper
{
    public class VoucherProfile : Profile
    {
        public VoucherProfile()
        {
            CreateMap<Voucher, VoucherResponse>();
        }
    }
}
