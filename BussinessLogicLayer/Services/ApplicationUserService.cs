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
            user.PhoneNumber = dto.PhoneNumber;

            // Update email if changed
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                user.Email = dto.Email;
                user.UserName = dto.Email;
            }

            // Update phone number
            user.PhoneNumber = dto.PhoneNumber;

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
                user.UserName = dto.Email; // Keep username synced with email
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

        public async Task<CollectorDto> HireCollectorAsync(HireCollectorDto dto)
        {
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already in use.");

            // Create new collector user
            var collector = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Points = 0,
                EmailConfirmed = true // Auto-confirm for hired collectors
            };

            var result = await _userManager.CreateAsync(collector, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create collector: {errors}");
            }

            // Assign Collector role
            await _userManager.AddToRoleAsync(collector, "Collector");

            // Get assigned orders count
            var orders = await _unitOfWork.Orders.GetOrdersByCollectorIdAsync(collector.Id);

            return new CollectorDto
            {
                Id = collector.Id,
                FullName = collector.FullName,
                Email = collector.Email ?? string.Empty,
                PhoneNumber = collector.PhoneNumber,
                AssignedOrdersCount = orders.Count(),
                HiredDate = DateTime.UtcNow
            };
        }

        public async Task<IEnumerable<CollectorDto>> GetAllCollectorsAsync()
        {
            // Get users in Collector role
            var collectorsInRole = await _userManager.GetUsersInRoleAsync("Collector");
            
            var collectorDtos = new List<CollectorDto>();

            foreach (var collector in collectorsInRole)
            {
                var orders = await _unitOfWork.Orders.GetOrdersByCollectorIdAsync(collector.Id);
                
                collectorDtos.Add(new CollectorDto
                {
                    Id = collector.Id,
                    FullName = collector.FullName,
                    Email = collector.Email ?? string.Empty,
                    PhoneNumber = collector.PhoneNumber,
                    AssignedOrdersCount = orders.Count(),
                    HiredDate = DateTime.UtcNow // You might want to add this to ApplicationUser
                });
            }

            return collectorDtos;
        }

        public async Task<CollectorDto?> GetCollectorByIdAsync(string collectorId)
        {
            var collector = await _userManager.FindByIdAsync(collectorId);
            if (collector == null)
                return null;

            // Verify user is a collector
            var isCollector = await _userManager.IsInRoleAsync(collector, "Collector");
            if (!isCollector)
                return null;

            var orders = await _unitOfWork.Orders.GetOrdersByCollectorIdAsync(collector.Id);

            return new CollectorDto
            {
                Id = collector.Id,
                FullName = collector.FullName,
                Email = collector.Email ?? string.Empty,
                PhoneNumber = collector.PhoneNumber,
                AssignedOrdersCount = orders.Count(),
                HiredDate = DateTime.UtcNow
            };
        }

        public async Task<bool> FireCollectorAsync(string collectorId)
        {
            var collector = await _userManager.FindByIdAsync(collectorId);
            if (collector == null)
                return false;

            // Check if user has assigned orders
            var orders = await _unitOfWork.Orders.GetOrdersByCollectorIdAsync(collectorId);
            var activeOrders = orders.Where(o => o.Status == OrderStatus.Pending).ToList();

            if (activeOrders.Any())
                throw new InvalidOperationException("Cannot fire collector with active pending orders. Please reassign orders first.");

            // Remove Collector role
            await _userManager.RemoveFromRoleAsync(collector, "Collector");

            // Optionally, you could delete the user entirely
            // await _userManager.DeleteAsync(collector);

            return true;
        }
    }
}
