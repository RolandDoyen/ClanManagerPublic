using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class ClanNotFoundException : Exception
    {
        public ClanNotFoundException()
            : base(Resources.Error_ClanNotFoundException) { }
    }
}
