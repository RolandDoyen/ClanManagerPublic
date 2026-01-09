using AutoMapper;
using ClanManager.BLL.DTO;
using ClanManager.DAL.DAO;

namespace ClanManager.BLL.Profiles
{
    /// <summary>
    /// AutoMapper profile configuration for clan-related entities, 
    /// defining the transformation rules between DAL data access objects and BLL data transfer objects.
    /// </summary>
    public class ClanProfileBLL : Profile
    {
        /// <summary>
        /// Initializes the mapping configurations for clans and their membership relations.
        /// </summary>
        public ClanProfileBLL()
        {
            /// <summary>
            /// Maps the <see cref="Clan"/> entity to its corresponding <see cref="ClanDTO"/>.
            /// Primarily used for retrieving clan details and list displays.
            /// </summary>
            CreateMap<Clan, ClanDTO>();

            /// <summary>
            /// Maps the <see cref="ClanMember"/> association entity to <see cref="ClanMemberDTO"/>.
            /// Preserves the relationship data between users and their respective clans.
            /// </summary>
            CreateMap<ClanMember, ClanMemberDTO>();
        }
    }
}