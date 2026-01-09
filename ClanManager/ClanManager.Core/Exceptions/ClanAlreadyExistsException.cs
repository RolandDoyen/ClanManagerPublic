namespace ClanManager.Core.Exceptions
{
    public class ClanAlreadyExistsException : ClanManagerBaseException
    {
        public ClanAlreadyExistsException()
            : base(Resources.Resources.Error_ClanAlreadyExistsException) { }
    }
}
