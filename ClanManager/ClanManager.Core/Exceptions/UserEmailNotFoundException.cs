namespace ClanManager.Core.Exceptions
{
    public class UserEmailNotFoundException : ClanManagerBaseException
    {
        public UserEmailNotFoundException()
            : base(Resources.Resources.Error_UserEmailNotFoundException) { }
    }
}
