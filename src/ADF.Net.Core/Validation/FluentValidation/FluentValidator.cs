using System.Collections.Generic;
using ADF.Net.Core.Helpers;
using FluentValidation;
using FluentValidation.Results;

namespace ADF.Net.Core.Validation.FluentValidation
{

    public class FluentValidator<TModel, TRules> : IValidator where TModel : class, new() where TRules : AbstractValidator<TModel>, new()
    {

        private readonly TModel _model;
        private TRules _rules;

        public bool IsValid { get; set; }

        protected readonly List<ValidationResult> ValidationResults;


        public FluentValidator(TModel model)
        {
            _model = model;
            ValidationResults = new List<ValidationResult>();
        }


        public List<ValidationResult> Validate()
        {
            _rules = new TRules();
            var results = _rules.Validate(_model);
            foreach (var error in results.Errors)
            {
                ValidationResults.Add(error.CreateMapped<ValidationFailure, ValidationResult>());
            }
            IsValid = results.IsValid;
            return ValidationResults;
        }
    }
}
