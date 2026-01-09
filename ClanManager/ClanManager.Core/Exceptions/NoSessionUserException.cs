namespace ClanManager.Core.Exceptions
{
    public class NoSessionUserException : ClanManagerBaseException
    {
        public NoSessionUserException()
            : base(Resources.Resources.Error_NoSessionUserException) { }
    }
}
