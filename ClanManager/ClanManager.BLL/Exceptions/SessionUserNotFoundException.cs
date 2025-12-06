using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class SessionUserNotFoundException : Exception
    {
        public SessionUserNotFoundException()
            : base(Resources.Error_SessionUserNotFoundException) { }
    }
}
