namespace ClanManager.Core.Exceptions
{
    public class SessionUserNotFoundException : ClanManagerBaseException
    {
        public SessionUserNotFoundException()
            : base(Resources.Resources.Error_SessionUserNotFoundException) { }
    }
}
