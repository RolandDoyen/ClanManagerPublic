using ClanManager.Core.Resources;

namespace ClanManager.BLL.Exceptions
{
    public class PasswordValidationException : Exception
    {
        public PasswordValidationException()
            : base(Resources.Error_PasswordValidationException) { }
    }
}
