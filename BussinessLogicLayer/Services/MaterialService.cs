using BusinessLogicLayer.DTOs;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;

namespace BusinessLogicLayer.Services
{
    public class MaterialService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MaterialService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ---------- helper mappers ----------
        private static MaterialDto ToDto(Material entity)
        {
            return new MaterialDto
            {
                ID = entity.ID,
                TypeName = entity.TypeName,
                Size = entity.Size.ToString(),        // double -> string
                Price = entity.Price,
                FactoryId = entity.FactoryId
            };
        }

        private static Material ToEntity(MaterialDto dto)
        {
            double sizeValue = 0;
            if (!string.IsNullOrWhiteSpace(dto.Size))
            {
                double.TryParse(dto.Size, out sizeValue);
            }

            return new Material
            {
                ID = dto.ID,
                TypeName = dto.TypeName,
                Size = sizeValue,                     // string -> double
                Price = dto.Price,
                FactoryId = dto.FactoryId
            };
        }

        private static void UpdateEntityFromDto(MaterialDto dto, Material entity)
        {
            entity.TypeName = dto.TypeName;

            double sizeValue = 0;
            if (!string.IsNullOrWhiteSpace(dto.Size))
            {
                double.TryParse(dto.Size, out sizeValue);
            }
            entity.Size = sizeValue;

            entity.Price = dto.Price;
            entity.FactoryId = dto.FactoryId;
        }

        // ---------- service methods ----------
        public async Task<IEnumerable<MaterialDto>> GetAllMaterialsAsync()
        {
            var materials = await _unitOfWork.Materials.GetAllAsync();
            return materials.Select(m => ToDto(m));
        }

        public async Task<MaterialDto?> GetMaterialByIdAsync(int id)
        {
            var material = await _unitOfWork.Materials.GetByIdAsync(id);
            return material == null ? null : ToDto(material);
        }

        public async Task<MaterialDto> CreateMaterialAsync(MaterialDto dto)
        {
            var entity = ToEntity(dto);

            await _unitOfWork.Materials.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<bool> UpdateMaterialAsync(int id, MaterialDto dto)
        {
            var material = await _unitOfWork.Materials.GetByIdAsync(id);
            if (material == null)
                return false;

            UpdateEntityFromDto(dto, material);

            _unitOfWork.Materials.Update(material);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var material = await _unitOfWork.Materials.GetByIdAsync(id);
            if (material == null)
                return false;

            _unitOfWork.Materials.Remove(material);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MaterialDto>> GetMaterialsByFactoryIdAsync(int factoryId)
        {
            var materials = await _unitOfWork.Materials.FindAsync(m => m.FactoryId == factoryId);
            return materials.Select(m => ToDto(m));
        }
    }
}
