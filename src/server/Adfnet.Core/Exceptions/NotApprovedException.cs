namespace Adfnet.Core.Exceptions
{

    /// <inheritdoc />
    /// <summary>
    /// Onaylanmamış kayıtlar için istisna sınıfı
    /// </summary>
    public class NotApprovedException : BaseApplicationException
    {
        public NotApprovedException(string message) : base(message) { }
    }
}