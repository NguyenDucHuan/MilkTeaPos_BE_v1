using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.CategoryModels;
using MilkTeaPosManagement.Api.Models.ViewModels;
using MilkTeaPosManagement.Api.Models.VoucherMethod;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Domain.Paginate;

namespace MilkTeaPosManagement.Api.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<IPaginate<Voucher>?> GetVouchersByFilterAsync(VoucherSearchModel? filter);
        Task<MethodResult<Voucher>> GetVoucherByIdAsync(int id);
        Task<MethodResult<Voucher>> CreateVoucherAsync(VoucherCreateRequestModel request);
        Task<MethodResult<Voucher>> UpdateVoucherAsync(int id, VoucherUpdateRequestModel request);
        Task<MethodResult<Voucher>> UpdateStatus(int id);
    }
}
