using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class NoSessionUserException : Exception
    {
        public NoSessionUserException()
            : base(Resources.Error_NoSessionUserException) { }
    }
}
