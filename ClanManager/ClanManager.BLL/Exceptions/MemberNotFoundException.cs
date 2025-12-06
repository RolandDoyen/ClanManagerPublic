using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class MemberNotFoundException : Exception
    {
        public MemberNotFoundException()
            : base(Resources.Error_MemberNotFoundException) { }
    }
}
