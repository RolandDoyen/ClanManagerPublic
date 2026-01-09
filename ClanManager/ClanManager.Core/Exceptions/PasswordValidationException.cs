namespace ClanManager.Core.Exceptions
{
    public class PasswordValidationException : ClanManagerBaseException
    {
        public PasswordValidationException()
            : base(Resources.Resources.Error_PasswordValidationException) { }
    }
}
