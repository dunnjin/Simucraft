using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public class EditContextValidator
    {
        private IDictionary<string, bool> _validationKeys = new Dictionary<string, bool>();

        public EditContextValidator(object model)
        {
            this.EditContext = new EditContext(model);
            this.EditContext.OnFieldChanged += this.HandleValidation;
        }

        public EditContext EditContext { get; protected set; }

        public bool IsFieldValidated(string fieldName)
        {
            if (!_validationKeys.ContainsKey(fieldName))
                _validationKeys.Add(fieldName, false);

            return !_validationKeys[fieldName];
        }

        public bool Validate()
        {
            var isValidated = this.EditContext.Validate();
            var keys = _validationKeys
                .Select(v => v.Key)
                .ToList();

            foreach (var field in keys)
                this.ValidateField(field);

            return isValidated;
        }

        public void NotifyField(string fieldName)
        {
            this.EditContext.NotifyFieldChanged(new FieldIdentifier(this.EditContext.Model, fieldName));
            this.ValidateField(fieldName);
        }

        private void HandleValidation(object sender, FieldChangedEventArgs fieldChangedEventArgs)
        {
            this.EditContext.Validate();
            this.ValidateField(fieldChangedEventArgs.FieldIdentifier.FieldName);
        }

        private void ValidateField(string fieldName)
        {
            if (!_validationKeys.ContainsKey(fieldName))
                _validationKeys.Add(fieldName, false);

            _validationKeys[fieldName] = this.EditContext
                .GetValidationMessages(this.EditContext.Field(fieldName))
                .Any();
        }
    }
}
