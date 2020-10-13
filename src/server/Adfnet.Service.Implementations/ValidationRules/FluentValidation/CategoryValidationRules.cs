﻿using Adfnet.Core.Globalization;
using Adfnet.Service.Models;
using FluentValidation;

namespace Adfnet.Service.Implementations.ValidationRules.FluentValidation
{
    public class CategoryValidationRules : AbstractValidator<CategoryModel>
    {
        public CategoryValidationRules()
        {
            RuleFor(p => p.Code).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Code));
            RuleFor(p => p.Name).NotEmpty().WithMessage(string.Format(Messages.DangerFieldIsEmpty, Dictionary.Name));
        }
    }
}
