using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.PaymentMethodModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.Api.ViewModels;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using System;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class PaymentMethodService(IUnitOfWork uow) : IPaymentmethodService
    {
        private readonly IUnitOfWork _uow = uow;
        public async Task<ICollection<Paymentmethod>> GetAllPaymentmethodAsync()
        {
            return await _uow.GetRepository<Paymentmethod>().GetListAsync();
        }
        public async Task<MethodResult<Paymentmethod>> GetPaymentmethodByIdAsync(int paymentmethodId)
        {
            var method = await _uow.GetRepository<Paymentmethod>().SingleOrDefaultAsync(
                predicate: pm => pm.PaymentMethodId == paymentmethodId);
            if (method == null)
            {
                return new MethodResult<Paymentmethod>.Failure("Payment method not found!", StatusCodes.Status400BadRequest);
            }
            return new MethodResult<Paymentmethod>.Success(method);
        }
        public async Task<MethodResult<Paymentmethod>> AddAPaymentmethodAsync(PaymentMethodRequest paymentmethodRequest)
        {
            var existed = await _uow.GetRepository<Paymentmethod>().GetListAsync(predicate: pm => pm.MethodName.ToLower().Equals(paymentmethodRequest.MethodName.ToLower()));
            if (existed != null && existed.Count > 0)
            {
                return new MethodResult<Paymentmethod>.Failure("Payment method with this name has already existed!", StatusCodes.Status400BadRequest);
            }
            if (await GetPaymentmethodByIdAsync(paymentmethodRequest.Id) != null)
            {
                return new MethodResult<Paymentmethod>.Failure("Payment method with this id has already existed!", StatusCodes.Status400BadRequest);
            }
            //var paymentMethods = await _uow.GetRepository<Paymentmethod>().GetListAsync();
            //var paymentMethodId = paymentMethods.Count > 0 ? paymentMethods.LastOrDefault().PaymentMethodId + 1 : 1;
            var paymentMethod = new Paymentmethod
            {
                PaymentMethodId = paymentmethodRequest.Id,
                MethodName = paymentmethodRequest.MethodName,
                Description = paymentmethodRequest.Description,
                Status = PaymentMethodConstant.PAYMENT_METHOD_STATUS_ACTIVE
            };
            await _uow.GetRepository<Paymentmethod>().InsertAsync(paymentMethod);
            return new MethodResult<Paymentmethod>.Success(paymentMethod);
        }
        public async Task<MethodResult<Paymentmethod>> UpdateAPaymentmethodAsync(PaymentMethodRequest paymentMethodRequest)
        {
            var existed = await _uow.GetRepository<Paymentmethod>().SingleOrDefaultAsync(
                predicate: pm => pm.PaymentMethodId == paymentMethodRequest.Id);
            if (existed == null)
            {
                return new MethodResult<Paymentmethod>.Failure("This payment method does not exist!", StatusCodes.Status400BadRequest);
            }
            existed.MethodName = paymentMethodRequest.MethodName;
            existed.Description = paymentMethodRequest.Description;
            _uow.GetRepository<Paymentmethod>().UpdateAsync(existed);
            return new MethodResult<Paymentmethod>.Success(existed);
        }
        public async Task<MethodResult<Paymentmethod>> DeleteAPaymentmethodAsync(int paymentmethodId)
        {
            var existed = await _uow.GetRepository<Paymentmethod>().SingleOrDefaultAsync(
                predicate: pm => pm.PaymentMethodId == paymentmethodId);
            if (existed == null)
            {
                return new MethodResult<Paymentmethod>.Failure("This payment method does not exist!", StatusCodes.Status400BadRequest);
            }
            existed.Status = PaymentMethodConstant.PAYMENT_METHOD_STATUS_INACTIVE;
            _uow.GetRepository<Paymentmethod>().UpdateAsync(existed);
            return new MethodResult<Paymentmethod>.Success(existed);
        }
    }
}
