using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.WEB.Models.Clan;

namespace ClanManager.WEB.Profiles
{
    public class ClanProfile : Profile
    {
        public ClanProfile()
        {
            CreateMap<ClanDTO, ClanViewModel>().ReverseMap();
            CreateMap<ClanUserContextDTO, ClanViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Clan.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Clan.Name))
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Clan.Tag))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Clan.Description))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Clan.IsActive))
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Clan.Members));

            CreateMap<ClanMemberDTO, ClanMemberViewModel>().ReverseMap();

            CreateMap<ClanMemberContextDTO, ClanMemberViewModel>();
        }
    }
}
