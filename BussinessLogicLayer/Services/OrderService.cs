using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.Order;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;
using DataAccessLayer.Utilities;

namespace BusinessLogicLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ---------- helper mappers ----------
        private static OrderDto ToDto(Order entity)
        {
            return new OrderDto
            {
                ID = entity.ID,
                Status = entity.Status.ToString(),
                OrderDate = entity.OrderDate,
                UserId = entity.UserId,
                CollectorId = entity.CollectorId,
                FactoryId = entity.FactoryId,
                UserName = entity.User?.UserName,
                CollectorName = entity.Collector?.UserName,
                FactoryName = entity.Factory?.Name
            };
        }

        private static Order ToEntity(OrderDto dto)
        {
            return new Order
            {
                ID = dto.ID,
                Status = Enum.Parse<OrderStatus>(dto.Status),
                OrderDate = dto.OrderDate,
                UserId = dto.UserId,
                CollectorId = dto.CollectorId,
                FactoryId = dto.FactoryId
            };
        }

        private static void UpdateEntityFromDto(OrderDto dto, Order entity)
        {
            entity.Status = Enum.Parse<OrderStatus>(dto.Status);
            entity.OrderDate = dto.OrderDate;
            entity.UserId = dto.UserId;
            entity.CollectorId = dto.CollectorId;
            entity.FactoryId = dto.FactoryId;
        }

        // ---------- service methods ----------
        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return orders.Select(o => ToDto(o));
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(id);
            return order == null ? null : ToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
            return orders.Select(o => ToDto(o));
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCollectorIdAsync(string collectorId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByCollectorIdAsync(collectorId);
            return orders.Select(o => ToDto(o));
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByFactoryIdAsync(int factoryId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByFactoryIdAsync(factoryId);
            return orders.Select(o => ToDto(o));
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
        {
            var orderStatus = Enum.Parse<OrderStatus>(status);
            var orders = await _unitOfWork.Orders.GetOrdersByStatusAsync(orderStatus);
            return orders.Select(o => ToDto(o));
        }

        public async Task<OrderDto> AddAsync(CreateOrderDto dto)
        {
            // Find user by email using the Users repository
            var allUsers = await _unitOfWork.Users.GetAllAsync();
            var appUser = allUsers.FirstOrDefault(u => u.Email == dto.Email);
            if (appUser == null)
                throw new KeyNotFoundException($"User with email {dto.Email} not found.");

            // Parse material type
            if (!Enum.TryParse<MaterialType>(dto.TypeOfMaterial, ignoreCase: true, out var materialType))
                throw new ArgumentException($"Invalid material type: {dto.TypeOfMaterial}");

            // Find or create material
            var materials = await _unitOfWork.Materials.GetMaterialsByTypeAsync(materialType.ToString());
            var material = materials.FirstOrDefault();
            
            if (material == null)
            {
                material = new Material
                {
                    TypeName = materialType.ToString(),
                    Size = dto.Quantity,
                    Price = 0 // Price can be set later or calculated based on business logic
                };
                await _unitOfWork.Materials.AddAsync(material);
            }
            else
            {
                material.Size = dto.Quantity;
            }

            // Find factory (for now, just get the first available factory - you may need better logic)
            var factories = await _unitOfWork.Factories.GetAllAsync();
            var factory = factories.FirstOrDefault();
            if (factory == null)
                throw new InvalidOperationException("No factory available.");

            // Create order entity
            var order = new Order
            {
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                Status = OrderStatus.Pending,
                UserId = appUser.Id,
                FactoryId = factory.ID,
                Materials = new List<Material> { material }
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Return the created order as DTO
            var createdOrder = await _unitOfWork.Orders.GetOrderWithDetailsAsync(order.ID);
            return ToDto(createdOrder!);
        }

        public async Task UpdateAsync(OrderDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(dto.ID);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {dto.ID} not found.");

            UpdateEntityFromDto(dto, order);
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found.");

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Completes an order and awards points to the user
        /// </summary>
        public async Task<bool> CompleteOrderAsync(int orderId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Get order with materials and user
                var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
                
                if (order == null)
                    throw new KeyNotFoundException($"Order with ID {orderId} not found.");

                if (order.Status != OrderStatus.Pending)
                    throw new InvalidOperationException("Only pending orders can be completed.");

                if (order.User == null)
                    throw new InvalidOperationException("Order has no associated user.");

                // Calculate points from materials
                int pointsEarned = PointsCalculator.CalculateOrderPoints(order);

                // Award points to user
                order.User.Points += pointsEarned;

                // Update order status
                order.Status = OrderStatus.Completed;

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Cancels an order (no points awarded)
        /// </summary>
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            
            if (order == null)
                return false;

            if (order.Status == OrderStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed order.");

            order.Status = OrderStatus.Cancelled;
            
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
