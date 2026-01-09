using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.DAL.DAO;

namespace ClanManager.BLL.Profiles
{
    /// <summary>
    /// AutoMapper profile configuration for mapping between User entities and UserDTOs.
    /// </summary>
    public class UserProfileBLL : Profile
    {
        /// <summary>
        /// Initializes mapping rules, ensuring sensitive information like passwords 
        /// and hashes are handled securely during conversion.
        /// </summary>
        public UserProfileBLL()
        {
            /// <summary>
            /// Maps a UserDTO to a User entity. 
            /// The PasswordHash is ignored to prevent accidental overwrites during updates.
            /// </summary>
            CreateMap<UserDTO, User>()
                    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                    .ForMember(dest => dest.Clans, opt => opt.Ignore());

            /// <summary>
            /// Maps a User entity to a UserDTO. 
            /// The Password string is ignored to ensure sensitive hash data is never exposed to the UI/BLL.
            /// </summary>
            CreateMap<User, UserDTO>()
                    .ForMember(dest => dest.Password, opt => opt.Ignore());
        }
    }
}
