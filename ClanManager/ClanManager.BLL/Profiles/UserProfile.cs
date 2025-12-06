using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.DAL.DAO;

namespace ClanManager.BLL.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDTO, User>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<User, UserDTO>().ForMember(dest => dest.Password, opt => opt.Ignore());
        }
    }
}
