using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class UserEmailNotFoundException : Exception
    {
        public UserEmailNotFoundException()
            : base(Resources.Error_UserEmailNotFoundException) { }
    }
}
