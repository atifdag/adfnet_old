using FluentValidation;
using Adfnet.Core.Globalization;
using Adfnet.Service.Models;

namespace Adfnet.Service.Implementations.ValidationRules.FluentValidation
{
    public class UserValidationRules : AbstractValidator<UserModel>
    {
        public UserValidationRules()
        {
            RuleFor(p => p.Username).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Username));
            RuleFor(p => p.Username).Length(8, 32).WithMessage(string.Format(Messages.DangerFieldLengthLimit, Dictionary.Username, "8"));
            RuleFor(p => p.Email).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Email));
            RuleFor(p => p.Email).EmailAddress().WithMessage(Messages.DangerInvalidEntitiy);
            RuleFor(p => p.FirstName).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.FirstName));
            RuleFor(p => p.LastName).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.LastName));
        }
    }
}
