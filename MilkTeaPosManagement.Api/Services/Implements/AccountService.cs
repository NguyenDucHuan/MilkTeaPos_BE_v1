
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.Domain.Models;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.ViewModels;
using AutoMapper;
using MilkTeaPosManagement.Api.Models.AccountModel;
using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Domain.Paginate;
using Microsoft.EntityFrameworkCore;
using System;
using MilkTeaPosManagement.Api.Extensions.Filter;


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

        public async Task<IPaginate<AccountViewModel>> GetAccountsByFilterAsync(AccountFilterModel filter)
        {
            if (filter == null)
                filter = new AccountFilterModel();
            var predicate = filter.BuildFilterExpression();
            return await _uow.GetRepository<Account>().GetPagingListAsync(
                selector: user => _mapper.Map<AccountViewModel>(user),
                predicate: predicate,
                orderBy: query => query.ApplySorting(filter.SortBy, filter.SortAscending),
                page: filter.Page,
                size: filter.PageSize
            );
        }

        public async Task<MethodResult<AccountViewModel>> GetAccountByIdAsync(int id)
        {
            var user = await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: u => u.AccountId == id

            );

            if (user == null)
            {
                return new MethodResult<AccountViewModel>.Failure("User not found", StatusCodes.Status404NotFound);
            }

            var userViewModel = _mapper.Map<AccountViewModel>(user);
            return new MethodResult<AccountViewModel>.Success(userViewModel);
        }

        public async Task<MethodResult<AccountViewModel>> CreateAccountAsync(CreateUserRequest request, IFormFile avatarFile)
        {
            try
            {
                var existingUser = await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                    predicate: u => u.Email == request.Email || u.Phone == request.Phone
                );

                if (existingUser != null)
                {
                    string field = existingUser.Email == request.Email ? "email" : "phone";
                    return new MethodResult<AccountViewModel>.Failure(
                        $"User with this {field} already exists",
                        StatusCodes.Status400BadRequest
                    );
                }
                string avatarUrl = null;
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    avatarUrl = await _cloudinaryService.UploadImageAsync(avatarFile);
                    if (string.IsNullOrEmpty(avatarUrl))
                    {
                        return new MethodResult<AccountViewModel>.Failure(
                            "Failed to upload avatar image",
                            StatusCodes.Status500InternalServerError
                        );
                    }
                }
                var user = new Account
                {
                    Username = request.Username,
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = request.Role,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = UserConstant.USER_STATUS_ACTIVE,
                    ImageUrl = avatarUrl
                };

                await _uow.GetRepository<Account>().InsertAsync(user);

                if (await _uow.CommitAsync() > 0)
                {
                    var userViewModel = _mapper.Map<AccountViewModel>(user);
                    return new MethodResult<AccountViewModel>.Success(userViewModel);
                }

                return new MethodResult<AccountViewModel>.Failure(
                    "Failed to create user",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<AccountViewModel>.Failure(
                    $"Error creating user: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<MethodResult<AccountViewModel>> UpdateAccountAsync(int id, UpdateUserRequest request, IFormFile avatarFile)
        {
            try
            {
                var user = await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                    predicate: u => u.AccountId == id
                );

                if (user == null)
                {
                    return new MethodResult<AccountViewModel>.Failure(
                        "User not found",
                        StatusCodes.Status404NotFound
                    );
                }
                if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
                {
                    var emailExists = await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                        predicate: u => u.Email == request.Email && u.AccountId != id
                    );

                    if (emailExists != null)
                    {
                        return new MethodResult<AccountViewModel>.Failure(
                            "Email is already in use",
                            StatusCodes.Status400BadRequest
                        );
                    }
                }
                if (!string.IsNullOrEmpty(request.Phone) && request.Phone != user.Phone)
                {
                    var phoneExists = await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                        predicate: u => u.Phone == request.Phone && u.AccountId != id
                    );

                    if (phoneExists != null)
                    {
                        return new MethodResult<AccountViewModel>.Failure(
                            "Phone number is already in use",
                            StatusCodes.Status400BadRequest
                        );
                    }
                }
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    string avatarUrl = await _cloudinaryService.UploadImageAsync(avatarFile);
                    if (string.IsNullOrEmpty(avatarUrl))
                    {
                        return new MethodResult<AccountViewModel>.Failure(
                            "Failed to upload avatar image",
                            StatusCodes.Status500InternalServerError
                        );
                    }
                    user.ImageUrl = avatarUrl;
                }
                if (!string.IsNullOrEmpty(request.Username))
                    user.Username = request.Username;
                if (!string.IsNullOrEmpty(request.FullName))
                    user.FullName = request.FullName;
                if (!string.IsNullOrEmpty(request.Email))
                    user.Email = request.Email;
                if (!string.IsNullOrEmpty(request.Phone))
                    user.Phone = request.Phone;
                if (!string.IsNullOrEmpty(request.Role))
                    user.Role = request.Role;
                if (request.Status.HasValue)
                    user.Status = request.Status.Value;

                user.UpdatedAt = DateTime.Now;

                _uow.GetRepository<Account>().UpdateAsync(user);
                if (await _uow.CommitAsync() > 0)
                {
                    var userViewModel = _mapper.Map<AccountViewModel>(user);
                    return new MethodResult<AccountViewModel>.Success(userViewModel);
                }

                return new MethodResult<AccountViewModel>.Failure(
                    "Failed to update user",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<AccountViewModel>.Failure(
                    $"Error updating user: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        public async Task<MethodResult<AccountViewModel>> UpdateAccountStatusAsync(int id)
        {
            try
            {
                var user = await _uow.GetRepository<Account>().SingleOrDefaultAsync(
                    predicate: u => u.AccountId == id
                );

                if (user == null)
                {
                    return new MethodResult<AccountViewModel>.Failure(
                        "User not found",
                        StatusCodes.Status404NotFound
                    );
                }
                if (user.Status == UserConstant.USER_STATUS_INACTIVE)
                {
                    user.Status = UserConstant.USER_STATUS_ACTIVE;
                }
                else
                {
                    user.Status = UserConstant.USER_STATUS_INACTIVE;
                }
                user.UpdatedAt = DateTime.Now;
                _uow.GetRepository<Account>().UpdateAsync(user);
                if (await _uow.CommitAsync() > 0)
                {
                    var userViewModel = _mapper.Map<AccountViewModel>(user);
                    return new MethodResult<AccountViewModel>.Success(userViewModel);
                }
                return new MethodResult<AccountViewModel>.Failure(
                    "Failed to disable user",
                    StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return new MethodResult<AccountViewModel>.Failure(
                    $"Error disabling user: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
