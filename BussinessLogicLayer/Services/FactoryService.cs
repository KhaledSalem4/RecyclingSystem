using AutoMapper;
using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.Factory;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class FactoryService : IFactoryService
    {
        private readonly DataAccessLayer.UnitOfWork.IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FactoryService(DataAccessLayer.UnitOfWork.IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FactoryDto>> GetAllFactoriesAsync()
        {
            var factories = await _unitOfWork.Factories.GetAllAsync();
            return _mapper.Map<IEnumerable<FactoryDto>>(factories);
        }

        public async Task<FactoryDto?> GetFactoryByIdAsync(int id)
        {
            var factory = await _unitOfWork.Factories.GetByIdAsync(id);
            return factory == null ? null : _mapper.Map<FactoryDto>(factory);
        }

        public async Task<FactoryDto> CreateFactoryAsync(CreateFactoryDto dto)
        {
            var factory = _mapper.Map<Factory>(dto);
            
            await _unitOfWork.Factories.AddAsync(factory);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FactoryDto>(factory);
        }

        public async Task<FactoryDto> UpdateFactoryAsync(UpdateFactoryDto dto)
        {
            var factory = await _unitOfWork.Factories.GetByIdAsync(dto.ID);
            
            if (factory == null)
            {
                throw new KeyNotFoundException($"Factory with ID {dto.ID} not found.");
            }

            _mapper.Map(dto, factory);
            
            _unitOfWork.Factories.Update(factory);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FactoryDto>(factory);
        }

        public async Task<bool> DeleteFactoryAsync(int id)
        {
            var factory = await _unitOfWork.Factories.GetByIdAsync(id);
            
            if (factory == null)
            {
                return false;
            }

            // Check if factory has active orders
            var orders = await _unitOfWork.Orders.FindAsync(o => o.FactoryId == id);
            var hasActiveOrders = orders.Any(o => o.Status == OrderStatus.Pending);

            if (hasActiveOrders)
            {
                throw new InvalidOperationException("Cannot delete factory with active pending orders.");
            }

            _unitOfWork.Factories.Remove(factory);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<FactoryDetailsDto?> GetFactoryWithOrdersAsync(int id)
        {
            var factory = await _unitOfWork.Factories.GetFactoryWithOrdersAsync(id);
            
            if (factory == null)
            {
                return null;
            }

            return _mapper.Map<FactoryDetailsDto>(factory);
        }
    }
}
