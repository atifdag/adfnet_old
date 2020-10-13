using FluentValidation;
using Adfnet.Core.Globalization;
using Adfnet.Service.Models;

namespace Adfnet.Service.Implementations.ValidationRules.FluentValidation
{
    public class PersonValidationRules : AbstractValidator<PersonModel>
    {
        public PersonValidationRules()
        {
            RuleFor(p => p.IdentityCode).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.IdentityCode));

            RuleFor(p => p.FirstName).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.FirstName));
            RuleFor(p => p.LastName).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.LastName));
        }
    }
}
