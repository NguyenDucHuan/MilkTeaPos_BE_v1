using AutoMapper;
using MilkTeaPosManagement.Api.Models.TransactionModels;
using MilkTeaPosManagement.Api.Models.VoucherMethod;
using MilkTeaPosManagement.Domain.Models;

namespace MilkTeaPosManagement.Api.Mapper
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionResponse>();
        }
    }
}
