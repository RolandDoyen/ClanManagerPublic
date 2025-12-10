using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class MemberAlreadyInClanException : Exception
    {
        public MemberAlreadyInClanException()
            : base(Resources.Error_MemberAlreadyInClanException) { }
    }
}
