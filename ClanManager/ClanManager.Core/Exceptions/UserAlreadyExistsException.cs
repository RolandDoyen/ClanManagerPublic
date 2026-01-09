namespace ClanManager.Core.Exceptions
{
    public class UserAlreadyExistsException : ClanManagerBaseException
    {
        public UserAlreadyExistsException()
            : base(Resources.Resources.Error_UserAlreadyExistsException) { }
    }
}
