using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Category;
using LearnCSharp.Application.Models.DTOs.User;
using LearnCSharp.Domain.Entities;
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
                CreatedDate = DateTime.Now,
            };
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user");
                }
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

        public async Task Update(Guid id, UpdateUserDTO model)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {id} not found.");
            }
            throw new NotImplementedException();
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