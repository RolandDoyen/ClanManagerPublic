namespace ClanManager.Core.Exceptions
{
    public class ClanInactiveException : ClanManagerBaseException
    {
        public ClanInactiveException()
            : base(Resources.Resources.Error_ClanInactiveException) { }
    }
}
