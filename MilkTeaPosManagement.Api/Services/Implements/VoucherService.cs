using AutoMapper;
using CloudinaryDotNet;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.ViewModels;
using MilkTeaPosManagement.Api.Models.VoucherMethod;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.Domain.Paginate;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class VoucherService(IUnitOfWork uow) : IVoucherService
    {
        private readonly IUnitOfWork _uow = uow;

        public async Task<IPaginate<Voucher>?> GetVouchersByFilterAsync(VoucherSearchModel? filter)
        {
            if (filter == null)
            {
                return await _uow.GetRepository<Voucher>().GetPagingListAsync(page: 1, size: 10, orderBy: o => o.OrderByDescending(v => v.CreatedAt));
            }
            return await _uow.GetRepository<Voucher>().GetPagingListAsync(predicate: v => (!string.IsNullOrEmpty(filter.VoucherCode) || v.VoucherCode.ToLower().Contains(filter.VoucherCode.ToLower())) &&
                                                                                          (!filter.MinDiscountAmount.HasValue || v.DiscountAmount > filter.MinDiscountAmount) &&
                                                                                          (!filter.MaxDiscountAmount.HasValue || v.DiscountAmount < filter.MaxDiscountAmount) &&
                                                                                          (!string.IsNullOrEmpty(filter.DiscountType) || v.DiscountType.ToLower().Contains(filter.DiscountType.ToLower())) &&
                                                                                          (!filter.FromDate.HasValue || v.ExpirationDate > filter.FromDate) &&
                                                                                          (!filter.ToDate.HasValue || v.ExpirationDate < filter.ToDate),
                                                                         page: filter.Page.HasValue ? (int)filter.Page : 1,
                                                                         size: filter.PageSize.HasValue ? (int)filter.PageSize : 10,
                                                                         orderBy: o => ((filter.SortAscending.HasValue && filter.SortAscending.Value) ? ((string.IsNullOrEmpty(filter.SortBy) || filter.SortBy.ToLower().Equals("createat")) ? o.OrderBy(od => od.CreatedAt)
                                                                                                                                                                                                                                            : filter.SortBy.ToLower().Equals("vouchercode") ? o.OrderBy(od => od.VoucherCode)
                                                                                                                                                                                                                                            : filter.SortBy.ToLower().Equals("discountamount") ? o.OrderBy(od => od.DiscountAmount)
                                                                                                                                                                                                                                            : filter.SortBy.ToLower().Equals("discounttype") ? o.OrderBy(od => od.DiscountType)
                                                                                                                                                                                                                                            : filter.SortBy.ToLower().Equals("expirationdate") ? o.OrderBy(od => od.ExpirationDate)
                                                                                                                                                                                                                                                                                                : o.OrderBy(od => od.MinimumOrderAmount))
                                                                                                                                                     : ((string.IsNullOrEmpty(filter.SortBy) || filter.SortBy.ToLower().Equals("createat")) ? o.OrderBy(od => od.CreatedAt)
                                                                                                                                                                                                                                            : filter.SortBy.ToLower().Equals("vouchercode") ? o.OrderByDescending(od => od.VoucherCode)
                                                                                                                                                                                                                                            : filter.SortBy.ToLower().Equals("discountamount") ? o.OrderByDescending(od => od.DiscountAmount)
                                                                                                                                                                                                                                            : filter.SortBy.ToLower().Equals("discounttype") ? o.OrderByDescending(od => od.DiscountType)
                                                                                                                                                                                                                                            : filter.SortBy.ToLower().Equals("expirationdate") ? o.OrderByDescending(od => od.ExpirationDate)
                                                                                                                                                                                                                                                                                                : o.OrderByDescending(od => od.MinimumOrderAmount))));
        }
        public async Task<MethodResult<Voucher>> GetVoucherByIdAsync(int id)
        {
            var voucher = await _uow.GetRepository<Voucher>().SingleOrDefaultAsync(
                predicate: c => c.VoucherId == id
            );

            if (voucher == null)
            {
                return new MethodResult<Voucher>.Failure("Voucher not found", StatusCodes.Status404NotFound);
            }

            return new MethodResult<Voucher>.Success(voucher);
        }
        public async Task<MethodResult<Voucher>> CreateVoucherAsync(VoucherCreateRequestModel request)
        {
            try
            {
                var existed = await _uow.GetRepository<Voucher>().SingleOrDefaultAsync(
                    predicate: c => c.VoucherCode.ToLower().Equals(request.VoucherCode.ToLower())
                );

                if (existed != null)
                {
                    return new MethodResult<Voucher>.Failure(
                        "Voucher with this code already existed",
                        StatusCodes.Status400BadRequest
                    );
                }
                if (request.DiscountType != DiscountTypeConstant.AMOUNT.ToString() && request.DiscountType != DiscountTypeConstant.PERCENTAGE.ToString())
                {
                    return new MethodResult<Voucher>.Failure("Discount type must be 'Amount' or 'Percentage'", StatusCodes.Status400BadRequest);
                }
                if (request.DiscountType == DiscountTypeConstant.AMOUNT.ToString() && request.DiscountAmount < 1)
                {
                    return new MethodResult<Voucher>.Failure(
                        "Discount amount must be more than 1",
                        StatusCodes.Status400BadRequest
                    );
                }
                if (request.DiscountType == DiscountTypeConstant.PERCENTAGE.ToString() && request.DiscountAmount > 1)
                {
                    return new MethodResult<Voucher>.Failure(
                        "Discount percent must be less than 1",
                        StatusCodes.Status400BadRequest
                    );
                }
                if (request.ExpirationDate < DateTime.Now)
                {
                    return new MethodResult<Voucher>.Failure(
                        "Expiration date cannot be in past",
                        StatusCodes.Status400BadRequest
                    );
                }

                var voucher = new Voucher
                {
                    VoucherCode = request.VoucherCode,
                    DiscountAmount = request.DiscountAmount,
                    DiscountType = request.DiscountType,
                    ExpirationDate = request.ExpirationDate,
                    MinimumOrderAmount = request.MinimumOrderAmount,
                    Status = true,
                    CreatedAt = DateTime.Now
                };

                await _uow.GetRepository<Voucher>().InsertAsync(voucher);

                if (await _uow.CommitAsync() > 0)
                {
                    return new MethodResult<Voucher>.Success(voucher);
                }

                return new MethodResult<Voucher>.Failure(
                    "Failed to create voucher",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<Voucher>.Failure(
                    $"Error creating voucher: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        public async Task<MethodResult<Voucher>> UpdateVoucherAsync(int id, VoucherUpdateRequestModel request)
        {
            try
            {
                var voucher = await _uow.GetRepository<Voucher>().SingleOrDefaultAsync(
                    predicate: c => c.VoucherId == id
                );

                if (voucher == null)
                {
                    return new MethodResult<Voucher>.Failure(
                        "Voucher not found",
                        StatusCodes.Status404NotFound
                    );
                }
                if (!string.IsNullOrEmpty(request.DiscountType) && request.DiscountType != DiscountTypeConstant.AMOUNT.ToString() && request.DiscountType != DiscountTypeConstant.PERCENTAGE.ToString())
                {
                    return new MethodResult<Voucher>.Failure("Discount type must be 'Amount' or 'Percentage'", StatusCodes.Status400BadRequest);
                }
                if (!string.IsNullOrEmpty(request.DiscountType) && request.DiscountAmount.HasValue && request.DiscountType == DiscountTypeConstant.AMOUNT.ToString() && request.DiscountAmount < 1)
                {
                    return new MethodResult<Voucher>.Failure(
                        "Discount amount must be more than 1",
                        StatusCodes.Status400BadRequest
                    );
                }
                if (!string.IsNullOrEmpty(request.DiscountType) && request.DiscountAmount.HasValue && request.DiscountType == DiscountTypeConstant.PERCENTAGE.ToString() && request.DiscountAmount > 1)
                {
                    return new MethodResult<Voucher>.Failure(
                        "Discount percent must be less than 1",
                        StatusCodes.Status400BadRequest
                    );
                }
                if (request.ExpirationDate.HasValue && request.ExpirationDate < DateTime.Now)
                {
                    return new MethodResult<Voucher>.Failure(
                        "Expiration date cannot be in past",
                        StatusCodes.Status400BadRequest
                    );
                }
                voucher.DiscountAmount = request.DiscountAmount.HasValue? request.DiscountAmount : voucher.DiscountAmount;
                voucher.DiscountType = !string.IsNullOrEmpty(request.DiscountType) ? request.DiscountType : voucher.DiscountType;
                voucher.ExpirationDate = request.ExpirationDate.HasValue? request.ExpirationDate : voucher.ExpirationDate;
                voucher.MinimumOrderAmount = request.MinimumOrderAmount.HasValue? request.MinimumOrderAmount : voucher.MinimumOrderAmount;
                voucher.UpdatedAt = DateTime.Now;

                _uow.GetRepository<Voucher>().UpdateAsync(voucher);

                if (await _uow.CommitAsync() > 0)
                {
                    return new MethodResult<Voucher>.Success(voucher);
                }

                return new MethodResult<Voucher>.Failure(
                    "Failed to update voucher",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<Voucher>.Failure(
                    $"Error updating voucher: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        public async Task<MethodResult<Voucher>> UpdateStatus(int id)
        {
            try
            {
                var voucher = await _uow.GetRepository<Voucher>().SingleOrDefaultAsync(
                    predicate: c => c.VoucherId == id
                );

                if (voucher == null)
                {
                    return new MethodResult<Voucher>.Failure(
                        "Voucher not found",
                        StatusCodes.Status404NotFound
                    );
                }
                voucher.Status = !voucher.Status;
                _uow.GetRepository<Voucher>().UpdateAsync(voucher);

                if (await _uow.CommitAsync() > 0)
                {
                    return new MethodResult<Voucher>.Success(voucher);
                }

                return new MethodResult<Voucher>.Failure(
                    "Failed to Update status voucher",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<Voucher>.Failure(
                    $"Error Update status voucher: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
