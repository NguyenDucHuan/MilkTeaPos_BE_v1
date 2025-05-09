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
        Task<IPaginate<VoucherResponse>?> GetVouchersByFilterAsync(VoucherSearchModel? filter);
        Task<ICollection<VoucherResponse>> GetActiveVouchersAsync();
        Task<MethodResult<VoucherResponse>> GetVoucherByIdAsync(int id);
        Task<MethodResult<VoucherResponse>> CreateVoucherAsync(VoucherCreateRequestModel request, int userId);
        Task<MethodResult<VoucherResponse>> UpdateVoucherAsync(int id, VoucherUpdateRequestModel request, int userId);
        Task<MethodResult<VoucherResponse>> UpdateStatus(int id, int userId);
    }
}
