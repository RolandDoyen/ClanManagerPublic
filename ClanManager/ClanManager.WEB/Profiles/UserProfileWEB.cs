using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.WEB.Models.User;

namespace ClanManager.WEB.Profiles
{
    /// <summary>
    /// AutoMapper profile configuration for the Web layer, 
    /// handling translations between BLL Data Transfer Objects and Web ViewModels.
    /// </summary>
    public class UserProfileWEB : Profile
    {
        /// <summary>
        /// Initializes the mapping configurations for user-related views.
        /// </summary>
        public UserProfileWEB()
        {
            /// <summary>
            /// Bidirectional mapping between UserDTO and UserDetailViewModel.
            /// Used for simple display and data retrieval.
            /// </summary>
            CreateMap<UserDTO, UserDetailViewModel>()
                    .ForMember(dest => dest.SessionUserRole, opt => opt.Ignore())
                    .ForMember(dest => dest.IsOwnProfile, opt => opt.Ignore())
                    .ForMember(dest => dest.IsUserSuperAdmin, opt => opt.Ignore())
                    .ForMember(dest => dest.CanManageUsers, opt => opt.Ignore())
                    .ForMember(dest => dest.CanBanUser, opt => opt.Ignore())
                    .ReverseMap();

            /// <summary>
            /// Maps UserContextDTO to UserDetailViewModel.
            /// Flattens the nested UserDTO properties and maps session-specific 
            /// permission flags to the flat structure of the ViewModel.
            /// </summary>
            CreateMap<UserContextDTO, UserDetailViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                    .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User.Role))
                    .ForMember(dest => dest.IsBanned, opt => opt.MapFrom(src => src.User.IsBanned));

            /// <summary>
            /// Bidirectional mapping between UserDTO and UserFormViewModel.
            /// Facilitates data flow for registration and login forms.
            /// </summary>
            CreateMap<UserDTO, UserFormViewModel>().ReverseMap();
        }
    }
}