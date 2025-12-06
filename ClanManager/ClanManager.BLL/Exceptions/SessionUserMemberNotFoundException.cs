using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class SessionUserMemberNotFoundException : Exception
    {
        public SessionUserMemberNotFoundException()
            : base(Resources.Error_SessionUserMemberNotFoundException) { }
    }
}
