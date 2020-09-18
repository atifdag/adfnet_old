namespace ADF.Net.Core.Validation
{

    /// <summary>
    /// Doğrulama sonucunu 
    /// </summary>
    public class ValidationResult
    {

        /// <summary>
        /// Doğrulanamayan özellik
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Doğrulama hata iletisi
        /// </summary>
        public string ErrorMessage { get; set; }

    }
}
