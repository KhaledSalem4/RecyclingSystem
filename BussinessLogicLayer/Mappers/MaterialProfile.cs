using AutoMapper;
using BusinessLogicLayer.DTOs;
using DataAccessLayer.Entities;


namespace BusinessLogicLayer.Mappers
{
    public class MaterialProfile : Profile
    {
        public MaterialProfile()
        {
            CreateMap<Material, MaterialDto>().ReverseMap();
        }
    }
}
