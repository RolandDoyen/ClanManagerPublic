using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
            : base(Resources.Error_UserNotFoundException) { }
    }
}
