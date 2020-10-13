using FluentValidation;
using Adfnet.Core.Globalization;
using Adfnet.Service.Models;

namespace Adfnet.Service.Implementations.ValidationRules.FluentValidation
{
    public class UpdatePasswordModelValidationRules : AbstractValidator<UpdatePasswordModel>
    {
        public UpdatePasswordModelValidationRules()
        {
            RuleFor(p => p.Password).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Password));
            //  RuleFor(p => p.Password).Must(password => password.ValidatePassword()).WithMessage(Messages.DangerInvalidPassword);
            RuleFor(p => p.ConfirmPassword).Equal(p => p.Password).WithMessage(Messages.DangerPasswordsDoNotMatch);
        }
    }
}
