using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class NotClanLeaderException : Exception
    {
        public NotClanLeaderException()
            : base(Resources.Error_NotClanLeaderException) { }
    }
}
