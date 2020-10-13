using System.Collections.Generic;
using Adfnet.Core.Validation;

namespace Adfnet.Core
{
    public interface IValidator
    {

        bool IsValid { get; set; }

        List<ValidationResult> Validate();
    }
}
