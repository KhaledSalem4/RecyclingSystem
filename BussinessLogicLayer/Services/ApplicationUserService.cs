using AutoMapper;
using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.AppUser;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogicLayer.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUserDto>> GetAllAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<ApplicationUserDto>>(users);
        }

        public async Task<ApplicationUserDto?> GetByIdAsync(string id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return null;

            return _mapper.Map<ApplicationUserDto>(user);
        }

        public async Task UpdateAsync(ApplicationUserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(dto.Id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {dto.Id} not found.");

            user.FullName = dto.FullName;
            user.Points = dto.Points;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            // Update FullName
            user.FullName = dto.FullName;

            // Update Email if changed
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(dto.Email);
                if (emailExists != null && emailExists.Id != userId)
                    throw new InvalidOperationException("Email already in use by another account.");

                user.Email = dto.Email;
                user.UserName = dto.FullName; // Keep username synced with email
                user.EmailConfirmed = false; // Require re-confirmation
            }

            // Update Phone Number
            if (dto.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = dto.PhoneNumber;
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<UpdateUserProfileDto?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            return new UpdateUserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Points = user.Points
            };
        }
    }
}
