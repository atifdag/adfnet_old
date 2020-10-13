using FluentValidation;
using Adfnet.Core.Globalization;
using Adfnet.Service.Models;

namespace Adfnet.Service.Implementations.ValidationRules.FluentValidation
{
    public class LoginModelValidationRules : AbstractValidator<LoginModel>
    {
        public LoginModelValidationRules()
        {
            RuleFor(p => p.Username).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Username));
            RuleFor(p => p.Password).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Password));
            RuleFor(p => p.Key).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Key));
        }
    }
}
