using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class ClanAlreadyExistsException : Exception
    {
        public ClanAlreadyExistsException()
            : base(Resources.Error_ClanAlreadyExistsException) { }
    }
}
