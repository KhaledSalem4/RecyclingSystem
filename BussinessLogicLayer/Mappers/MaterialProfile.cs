using AutoMapper;
using BussinessLogicLayer.DTOs.Material;
using DataAccessLayer.Entities;


namespace BusinessLogicLayer.Mappers
{
    public class MaterialProfile : Profile
    {
        public MaterialProfile()
        {
            CreateMap<Material, MaterialDto>()
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => (int?)src.Size));
            
            CreateMap<MaterialDto, Material>()
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => (double)(src.Size ?? 0)));
        }
    }
}
