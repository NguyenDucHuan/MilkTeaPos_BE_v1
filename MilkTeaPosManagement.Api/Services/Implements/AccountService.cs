
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.ViewModels;
using AutoMapper;
using MilkTeaPosManagement.Api.Models.AccountModel;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;


        public AccountService(IUnitOfWork uow, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _uow = uow;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<MethodResult<bool>> ChangePasswordAsync(string email, ChangePasswordRequest request)
        {
            var user = await _uow.GetRepository<Account>().SingleOrDefaultAsync(
            predicate: p => p.Email == email
                 );

            if (user == null)
            {
                return new MethodResult<bool>.Failure("User not found", StatusCodes.Status404NotFound);
            }

            var passwordVerified = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);
            if (!passwordVerified)
            {
                return new MethodResult<bool>.Failure("Current password is incorrect", StatusCodes.Status400BadRequest);
            }

            if (request.CurrentPassword == request.NewPassword)
            {
                return new MethodResult<bool>.Failure("New password cannot be the same as current password",
                    StatusCodes.Status400BadRequest);
            }

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            user.PasswordHash = newPasswordHash;

            _uow.GetRepository<Account>().UpdateAsync(user);
            await _uow.CommitAsync();

            return new MethodResult<bool>.Success(true);
        }

        public async Task<MethodResult<ProfileViewModel>> GetProfileAsync(string email)
        {
            var user = await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                     predicate: p => p.Email == email
                 );

            var result = _mapper.Map<ProfileViewModel>(user);
            return new MethodResult<ProfileViewModel>.Success(result);
        }

        public async Task<Account> GetUserByPhoneOrEmailAsync(string PhoneOrEmail)
        {
            return await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: p => p.Email == PhoneOrEmail || p.Phone == PhoneOrEmail
            );
        }
        public async Task<MethodResult<string>> UpdateAvatarAsync(string email, IFormFile avatarFile)
        {
            if (avatarFile == null || avatarFile.Length == 0)
            {
                return new MethodResult<string>.Failure("No avatar file provided", StatusCodes.Status400BadRequest);
            }

            var user = await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: p => p.Email == email
            );

            if (user == null)
            {
                return new MethodResult<string>.Failure("User not found", StatusCodes.Status404NotFound);
            }

            try
            {
                string avatarUrl = await _cloudinaryService.UploadImageAsync(avatarFile);

                if (string.IsNullOrEmpty(avatarUrl))
                {
                    return new MethodResult<string>.Failure("Failed to upload avatar", StatusCodes.Status500InternalServerError);
                }
                user.ImageUrl = avatarUrl;
                _uow.GetRepository<Account>().UpdateAsync(user);
                await _uow.CommitAsync();
                return new MethodResult<string>.Success(avatarUrl);
            }
            catch (Exception ex)
            {
                return new MethodResult<string>.Failure($"Avatar update failed: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

    }
}
