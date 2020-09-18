using System.Collections.Generic;
using ADF.Net.Core.Validation;

namespace ADF.Net.Core.Exceptions
{



    public class ValidationException : BaseApplicationException
    {
        private List<ValidationResult> _validationResult;

        public List<ValidationResult> ValidationResult
        {
            get
            {
                var list = _validationResult;
                if (list != null)
                {
                    return list;
                }

                return (_validationResult = new List<ValidationResult>());
            }
            set => _validationResult = value;
        }

        public ValidationException(string message) : base(message) { }
    }
}