namespace ClanManager.Core.Exceptions
{
    public class WrongRoleException : ClanManagerBaseException
    {
        public WrongRoleException()
            : base(Resources.Resources.Error_WrongRoleException) { }
    }
}
