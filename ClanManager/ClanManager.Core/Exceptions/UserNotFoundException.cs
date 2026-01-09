namespace ClanManager.Core.Exceptions
{
    public class UserNotFoundException : ClanManagerBaseException
    {
        public UserNotFoundException()
            : base(Resources.Resources.Error_UserNotFoundException) { }
    }
}
