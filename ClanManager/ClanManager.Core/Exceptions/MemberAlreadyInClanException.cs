namespace ClanManager.Core.Exceptions
{
    public class MemberAlreadyInClanException : ClanManagerBaseException
    {
        public MemberAlreadyInClanException()
            : base(Resources.Resources.Error_MemberAlreadyInClanException) { }
    }
}
