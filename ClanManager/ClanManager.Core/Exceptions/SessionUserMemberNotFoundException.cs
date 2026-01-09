namespace ClanManager.Core.Exceptions
{
    public class SessionUserMemberNotFoundException : ClanManagerBaseException
    {
        public SessionUserMemberNotFoundException()
            : base(Resources.Resources.Error_SessionUserMemberNotFoundException) { }
    }
}
