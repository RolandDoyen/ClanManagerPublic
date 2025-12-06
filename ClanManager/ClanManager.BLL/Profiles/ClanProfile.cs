using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.DAL.DAO;

namespace ClanManager.BLL.Profiles
{
    public class ClanProfile : Profile
    {
        public ClanProfile()
        {
            CreateMap<Clan, ClanDTO>();
            CreateMap<ClanMember, ClanMemberDTO>();
        }
    }
}
