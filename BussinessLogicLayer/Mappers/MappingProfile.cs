using AutoMapper;
using BussinessLogicLayer.DTOs.AppDto;
using BussinessLogicLayer.DTOs.FactoryDto;
using BussinessLogicLayer.DTOs.HistoryReward;
using BussinessLogicLayer.DTOs.Material;
using BussinessLogicLayer.DTOs.OrderDto;
using BussinessLogicLayer.DTOs.Reward;
using DataAccessLayer.Entities;
using RecyclingSystem.DataAccess.Entities;

namespace BusinessLogicLayer.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Factory mappings
            CreateMap<Factory, FactoryDto>().ReverseMap();
            CreateMap<Factory, CreateFactoryDto>().ReverseMap();
            CreateMap<Factory, UpdateFactoryDto>().ReverseMap();
            CreateMap<Factory, FactoryDetailsDto>()
                .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<OrderStatus>(src.Status)));

            // Material mappings
            CreateMap<Material, MaterialDto>().ReverseMap();

            // Reward mappings
            CreateMap<Reward, RewardDto>().ReverseMap();

            // HistoryReward mappings
            CreateMap<HistoryReward, HistoryRewardDto>().ReverseMap();

            // ApplicationUser mappings
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
        }
    }
}
