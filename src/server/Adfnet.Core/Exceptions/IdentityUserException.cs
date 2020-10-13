namespace Adfnet.Core.Exceptions
{

    /// <inheritdoc />
    /// <summary>
    /// Kimlik kullanıcı istisna işlemlerinde kullanılacak sınıf
    /// </summary>
    public class IdentityUserException : BaseApplicationException
    {
        public IdentityUserException(string message) : base(message)
        {

        }
    }
}