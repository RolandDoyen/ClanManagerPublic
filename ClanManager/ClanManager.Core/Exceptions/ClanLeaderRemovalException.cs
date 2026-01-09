namespace ClanManager.Core.Exceptions
{
    public class ClanLeaderRemovalException : ClanManagerBaseException
    {
        public ClanLeaderRemovalException()
            : base(Resources.Resources.Error_ClanLeaderRemovalException) { }
    }
}
