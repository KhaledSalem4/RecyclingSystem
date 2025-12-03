using AutoMapper;
using BussinessLogicLayer.DTOs.Material;
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
