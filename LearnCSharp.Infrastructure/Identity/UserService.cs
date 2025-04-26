using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.User;
using LearnCSharp.Application.Utility;
using LearnCSharp.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LearnCSharp.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteAsync(Guid id, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            user.IsActive = isActive;
            user.UpdatedDate = DateTime.Now;
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task CreateAsync(CreateUserDTO model)
        {
            if (await CheckEmailExitsAsync(model.Email))
            {
                throw new Exception("Email already in use");
            }
            if (await CheckUserNameExitsAsync(model.UserName))
            {
                throw new Exception("UserName already in use");
            }

            var user = new AppUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Dob = model.Dob,
                Address = model.Address,
                FullName = model.FullName,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
            };
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user");
                }
                await _userManager.AddToRoleAsync(user, SD.RoleCustomer);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var data = new UserDTO()
            {
                Id = id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                Address = user.Address,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate,
                IsActive = user.IsActive,
                Dob = user.Dob,
            };
            return data;
        }

        public async Task UpdateAsync(Guid id, UpdateUserDTO model)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {id} not found.");
            }
            user.UserName = model.UserName ?? user.UserName;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
            user.FullName = model.FullName ?? user.FullName;
            user.Address = model.Address ?? user.Address;
            user.UpdatedDate = DateTime.UtcNow;
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to update user");
                }
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task ChangePasswordAsync(Guid id, ChangePasswordDTO model)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {id} not found.");
            }
            var passWordCurrent = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!passWordCurrent)
            {
                throw new Exception("Current password is incorrect");
            }
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                throw new Exception("New password and confirm password do not match");
            }
            user.UpdatedDate = DateTime.Now;
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to change password");
                }
                await _userManager.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task<bool> CheckEmailExitsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        private async Task<bool> CheckUserNameExitsAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user != null;
        }
    }
}