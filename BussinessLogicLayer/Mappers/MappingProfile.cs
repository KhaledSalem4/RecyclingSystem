using AutoMapper;
using BussinessLogicLayer.DTOs.AppUser;
using BussinessLogicLayer.DTOs.Factory;
using BussinessLogicLayer.DTOs.HistoryReward;
using BussinessLogicLayer.DTOs.Material;
using BussinessLogicLayer.DTOs.Order;
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
            CreateMap<ApplicationUser, ApplicationUserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ReverseMap();
            CreateMap<ApplicationUser, UpdateUserProfileDto>().ReverseMap();
            CreateMap<UpdateUserDto, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Points, opt => opt.Ignore());

            // Reward mappings
            // Reward mappings
            CreateMap<Reward, RewardDto>();

            CreateMap<CreateRewardDto, Reward>()
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.StockQuantity > 0));

            CreateMap<UpdateRewardDto, Reward>()
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.StockQuantity > 0));

            CreateMap<Reward, UpdateRewardDto>();
            CreateMap<Reward, RewardWithStatsDto>();

        }
    }
}
