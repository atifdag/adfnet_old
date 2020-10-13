using FluentValidation;
using Adfnet.Core.Globalization;
using Adfnet.Service.Models;

namespace Adfnet.Service.Implementations.ValidationRules.FluentValidation
{
    public class ParameterValidationRules : AbstractValidator<ParameterModel>
    {
        public ParameterValidationRules()
        {
            RuleFor(p => p.Key).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Key));
            RuleFor(p => p.Value).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Value));
        }
    }
}
