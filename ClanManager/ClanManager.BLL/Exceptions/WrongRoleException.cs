using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class WrongRoleException : Exception
    {
        public WrongRoleException()
            : base(Resources.Error_WrongRoleException) { }
    }
}
