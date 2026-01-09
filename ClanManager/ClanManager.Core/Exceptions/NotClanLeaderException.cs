namespace ClanManager.Core.Exceptions
{
    public class NotClanLeaderException : ClanManagerBaseException
    {
        public NotClanLeaderException()
            : base(Resources.Resources.Error_NotClanLeaderException) { }
    }
}
