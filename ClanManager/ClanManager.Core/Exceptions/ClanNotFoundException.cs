namespace ClanManager.Core.Exceptions
{
    public class ClanNotFoundException : ClanManagerBaseException
    {
        public ClanNotFoundException()
            : base(Resources.Resources.Error_ClanNotFoundException) { }
    }
}
