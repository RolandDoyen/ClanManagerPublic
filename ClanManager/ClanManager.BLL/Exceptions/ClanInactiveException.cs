using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class ClanInactiveException : Exception
    {
        public ClanInactiveException()
            : base(Resources.Error_ClanInactiveException) { }
    }
}
