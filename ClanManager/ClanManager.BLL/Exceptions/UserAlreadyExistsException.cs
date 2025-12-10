using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException()
            : base(Resources.Error_UserAlreadyExistsException) { }
    }
}
