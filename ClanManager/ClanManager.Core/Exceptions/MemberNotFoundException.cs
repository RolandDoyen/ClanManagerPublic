namespace ClanManager.Core.Exceptions
{
    public class MemberNotFoundException : ClanManagerBaseException
    {
        public MemberNotFoundException()
            : base(Resources.Resources.Error_MemberNotFoundException) { }
    }
}
