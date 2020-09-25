using ADF.Net.Core.Globalization;
using ADF.Net.Service.Models;
using FluentValidation;

namespace ADF.Net.Service.Implementations.ValidationRules.FluentValidation
{
    public class ProductValidationRules : AbstractValidator<ProductModel>
    {
        public ProductValidationRules()
        {
            RuleFor(p => p.Code).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Code));
            RuleFor(p => p.Name).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Name));
            RuleFor(p => p.UnitPrice).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.UnitPrice));
            RuleFor(p => p.Category.Item1).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Category));
        }
    }
}
