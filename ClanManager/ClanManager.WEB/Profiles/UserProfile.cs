using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.WEB.Models.User;

namespace ClanManager.WEB.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDTO, UserDetailViewModel>().ReverseMap();
            CreateMap<UserContextDTO, UserDetailViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User.Role))
                .ForMember(dest => dest.IsBanned, opt => opt.MapFrom(src => src.User.IsBanned));

            CreateMap<UserDTO, UserFormViewModel>().ReverseMap();
        }
    }
}
