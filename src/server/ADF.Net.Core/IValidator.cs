using System.Collections.Generic;
using ADF.Net.Core.Validation;

namespace ADF.Net.Core
{
    public interface IValidator
    {

        bool IsValid { get; set; }

        List<ValidationResult> Validate();
    }
}
