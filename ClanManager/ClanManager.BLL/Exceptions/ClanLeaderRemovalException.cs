using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class ClanLeaderRemovalException : Exception
    {
        public ClanLeaderRemovalException()
            : base(Resources.Error_ClanLeaderRemovalException) { }
    }
}
