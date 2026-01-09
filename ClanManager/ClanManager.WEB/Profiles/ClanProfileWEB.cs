using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.WEB.Models.Clan;

namespace ClanManager.WEB.Profiles
{
    /// <summary>
    /// AutoMapper profile for the Web layer, defining transformations between BLL Data Transfer Objects and UI ViewModels.
    /// </summary>
    public class ClanProfileWEB : Profile
    {
        /// <summary>
        /// Initializes the mapping configurations for clan views and member management.
        /// </summary>
        public ClanProfileWEB()
        {
            /// <summary>
            /// Standard bidirectional mapping between <see cref="ClanDTO"/> and <see cref="ClanViewModel"/>.
            /// </summary>
            CreateMap<ClanDTO, ClanViewModel>()
                    .ForMember(dest => dest.IsSessionUserMember, opt => opt.Ignore())
                    .ForMember(dest => dest.IsSessionUserLeader, opt => opt.Ignore())
                    .ForMember(dest => dest.IsSessionUserAdmin, opt => opt.Ignore())
                    .ForMember(dest => dest.CanSessionUserJoinQuit, opt => opt.Ignore())
                    .ForMember(dest => dest.LeaderEmail, opt => opt.Ignore())
                    .ForMember(dest => dest.IsSessionUserCoLeader, opt => opt.Ignore())
                    .ForMember(dest => dest.SessionUserId, opt => opt.Ignore())
                    .ForMember(dest => dest.IsEditingDescription, opt => opt.Ignore())
                    .ReverseMap();

            /// <summary>
            /// Advanced mapping from <see cref="ClanUserContextDTO"/> to <see cref="ClanViewModel"/>.
            /// Flattens the nested Clan DTO properties directly into the root ViewModel for easier UI binding.
            /// </summary>
            CreateMap<ClanUserContextDTO, ClanViewModel>()
                    .IncludeMembers(src => src.Clan) // Optionnel mais propre
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Clan.Id))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Clan.Name))
                    .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Clan.Tag))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Clan.Description))
                    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Clan.IsActive))
                    .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Clan.Members))
                    .ForMember(dest => dest.IsEditingDescription, opt => opt.Ignore()); // Propriété UI pure

            /// <summary>
            /// Bidirectional mapping for member records, preserving historical and role data.
            /// </summary>
            CreateMap<ClanMemberDTO, ClanMemberViewModel>()
                    .ForMember(dest => dest.CanChangeMemberRole, opt => opt.Ignore())
                    .ForMember(dest => dest.CanRemoveMember, opt => opt.Ignore())
                    .ReverseMap();

            /// <summary>
            /// Maps contextual member data (including calculated permission flags) to the member view model.
            /// </summary>
            CreateMap<ClanMemberContextDTO, ClanMemberViewModel>();
        }
    }
}