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
                
                // Material type and quantity mapping
                TypeOfMaterial = entity.TypeOfMaterial.ToString(),
                Quantity = entity.Quantity,
                
                // ✅ FIX: Use FullName first, then UserName, then Email as fallback
                UserName = entity.User?.FullName ?? entity.User?.UserName ?? entity.User?.Email,
                CollectorName = entity.Collector?.FullName ?? entity.Collector?.UserName ?? entity.Collector?.Email,
                FactoryName = entity.Factory?.Name,
                
                // Pickup Address (from Order itself)
                City = entity.City,
                Street = entity.Street,
                BuildingNo = entity.BuildingNo,
                Apartment = entity.Apartment,
                
                // User Address
                UserCity = entity.User?.City,
                UserStreet = entity.User?.Street,
                UserBuildingNo = entity.User?.BuildingNo,
                UserApartment = entity.User?.Apartment,
                
                // Collector Address
                CollectorCity = entity.Collector?.City,
                CollectorStreet = entity.Collector?.Street,
                CollectorBuildingNo = entity.Collector?.BuildingNo,
                CollectorApartment = entity.Collector?.Apartment,
                
                // Factory Address
                FactoryCity = entity.Factory?.City,
                FactoryStreet = entity.Factory?.Street,
                FactoryBuildingNo = entity.Factory?.BuildingNo,
                FactoryArea = entity.Factory?.Area
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

        // ---------- existing service methods ----------
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
            // 1️⃣ Find user by email
            var allUsers = await _unitOfWork.Users.GetAllAsync();
            var appUser = allUsers.FirstOrDefault(u => u.Email == dto.Email);

            if (appUser == null)
                throw new KeyNotFoundException($"User with email {dto.Email} not found.");

            // 2️⃣ Update user address if not already set
            if (string.IsNullOrEmpty(appUser.City) && !string.IsNullOrEmpty(dto.City))
            {
                appUser.City = dto.City;
                appUser.Street = dto.Street;
                appUser.BuildingNo = dto.BuildingNo;
                appUser.Apartment = dto.Apartment;

                _unitOfWork.Users.Update(appUser);
            }

            // 3️⃣ Find factory
            var factories = await _unitOfWork.Factories.GetAllAsync();
            var factory = factories.FirstOrDefault();

            if (factory == null)
                throw new InvalidOperationException("No factory available.");

            // 4️⃣ Create order
            var order = new Order
            {
                OrderDate = DateTime.Now, // ✅ Changed from DateOnly to DateTime
                Status = OrderStatus.Pending,

                UserId = appUser.Id,
                FactoryId = factory.ID,

                City = dto.City,
                Street = dto.Street,
                BuildingNo = dto.BuildingNo,
                Apartment = dto.Apartment,

                TypeOfMaterial = dto.TypeOfMaterial,
                Quantity = dto.Quantity
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

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

        public async Task<bool> CompleteOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            if (order.Status != OrderStatus.Delivered)
                throw new InvalidOperationException("Only delivered orders can be completed.");

            if (order.User == null)
                throw new InvalidOperationException("Order has no associated user.");

            // 1️⃣ Calculate points
            int pointsEarned = PointsCalculator.CalculateOrderPoints(order);

            // 2️⃣ Award points to user
            order.User.Points += pointsEarned;
            
            // ✅ FIX: Explicitly update user to track points change
            _unitOfWork.Users.Update(order.User);

            // 3️⃣ Update order status
            order.Status = OrderStatus.Completed;
            _unitOfWork.Orders.Update(order);

            // 4️⃣ Save both User and Order changes (atomic)
            await _unitOfWork.SaveChangesAsync();

            return true;
        }


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

        /// <summary>
        /// User cancels their own order
        /// </summary>
        public async Task<bool> UserCancelOrderAsync(int orderId, string userId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            // Verify user owns this order
            if (order.UserId != userId)
                throw new UnauthorizedAccessException("You can only cancel your own orders.");

            // Check if order can be cancelled
            if (order.Status == OrderStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed order.");

            if (order.Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel an order that has been delivered.");

            order.Status = OrderStatus.Cancelled;
            
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // ---------- NEW COLLECTOR METHODS ----------

        /// <summary>
        /// Collector accepts/picks up an available order (self-assignment)
        /// </summary>
        public async Task<bool> CollectorAcceptOrderAsync(int orderId, string collectorId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be accepted.");

            // Verify collector exists
            var collector = await _unitOfWork.Users.GetByIdAsync(collectorId);
            if (collector == null)
                throw new KeyNotFoundException("Collector not found.");

            order.CollectorId = collectorId;
            order.Status = OrderStatus.Accepted;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Collector updates order status (Assigned -> InProgress -> Delivered)
        /// </summary>
        public async Task<bool> CollectorUpdateOrderStatusAsync(int orderId, string collectorId, string newStatus)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            // Verify collector owns this order
            if (order.CollectorId != collectorId)
                throw new UnauthorizedAccessException("You can only update your own orders.");

            // Parse new status
            if (!Enum.TryParse<OrderStatus>(newStatus, true, out var newOrderStatus))
                throw new ArgumentException($"Invalid status: {newStatus}");

            // Validate status transitions
            var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
                {
                    { OrderStatus.Accepted, new List<OrderStatus> { OrderStatus.Collected, OrderStatus.Cancelled } },
                    { OrderStatus.Collected, new List<OrderStatus> { OrderStatus.Delivered, OrderStatus.Cancelled } },
                    { OrderStatus.Delivered, new List<OrderStatus> { OrderStatus.Completed } }
                };

            if (!validTransitions.ContainsKey(order.Status) ||
                !validTransitions[order.Status].Contains(newOrderStatus))
            {
                throw new InvalidOperationException(
                    $"Cannot transition from {order.Status} to {newOrderStatus}");
            }

            order.Status = newOrderStatus;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Get all pending orders available for collectors to accept
        /// </summary>
        public async Task<IEnumerable<OrderDto>> GetAvailableOrdersForCollectorsAsync()
        {
            var orders = await _unitOfWork.Orders.GetOrdersByStatusAsync(OrderStatus.Pending);
            return orders.Where(o => o.CollectorId == null).Select(o => ToDto(o));
        }

        /// <summary>
        /// Get collector's own orders
        /// </summary>
        public async Task<IEnumerable<OrderDto>> GetMyOrdersAsCollectorAsync(string collectorId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByCollectorIdAsync(collectorId);
            return orders.Select(o => ToDto(o));
        }
    }
}
