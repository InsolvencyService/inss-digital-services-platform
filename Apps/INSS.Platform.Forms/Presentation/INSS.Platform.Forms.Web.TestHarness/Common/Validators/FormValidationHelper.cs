using INSS.Platform.Forms.Web.TestHarness.Common.Services;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace INSS.Platform.Forms.Web.TestHarness.Common.Validators
{
    public static class FormValidationHelper
    {
        public static bool ValidateAndAssignFormProperty<TForm>(
            IPropertyValidator propertyValidator,
            EditContext editContext,
            TForm formInstance,
            TForm formToValidate,
            TForm formToPersist,
            string property)
            where TForm : class
        {
            object? value = typeof(TForm).GetProperty(property)?.GetValue(formToValidate);

            IList<ValidationResult> errors = propertyValidator.ValidateProperties(formToValidate, [property]);
            if (errors.Any())
            {
                ValidationMessageStore validationMessageStore = new(editContext);
                foreach (ValidationResult error in errors)
                {
                    FieldIdentifier fieldIdentifier = new(formInstance, error.MemberNames.First());
                    validationMessageStore.Add(fieldIdentifier, error.ErrorMessage!);
                }

                editContext.NotifyValidationStateChanged();
                SetPropertyValueByName(formInstance, property, value);
                return false;
            }

            SetPropertyValueByName(formToPersist, property, value);
            return true;
        }

        public static void SetPropertyValueByName(object target, string propertyName, object? value)
        {
            PropertyInfo? property = target.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(target, value);
            }
        }

        public static bool ValidateAndAssignFormProperty<TForm, TComplex>(
            IPropertyValidator propertyValidator,
            EditContext editContext,
            TForm formInstance,
            TComplex complexPropertyToValidate,
            TForm formToPersist,
            string propertyName)
            where TForm : class
            where TComplex : class
        {
            IList<ValidationResult> errors = propertyValidator.ValidateAllProperties(complexPropertyToValidate);
            if (errors.Any())
            {
                ValidationMessageStore validationMessageStore = new(editContext);

                object? formComplexInstance = typeof(TForm).GetProperty(propertyName)?.GetValue(formInstance);
                if (formComplexInstance is not { })
                {
                    throw new InvalidOperationException("The property to validate must exist on the bound instance of the Form property.");
                }

                foreach (ValidationResult error in errors)
                {
                    foreach (string memberName in error.MemberNames)
                    {
                        FieldIdentifier fieldIdentifier = new(formComplexInstance, memberName);
                        validationMessageStore.Add(fieldIdentifier, error.ErrorMessage!);
                    }
                }

                editContext.NotifyValidationStateChanged();

                foreach (PropertyInfo prop in typeof(TComplex).GetProperties())
                {
                    object? value = prop.GetValue(complexPropertyToValidate);
                    prop.SetValue(formComplexInstance, value);
                }

                return false;
            }

            object? targetComplexInstance = typeof(TForm).GetProperty(propertyName)?.GetValue(formToPersist);
            foreach (PropertyInfo prop in typeof(TComplex).GetProperties())
            {
                object? value = prop.GetValue(complexPropertyToValidate);
                prop.SetValue(targetComplexInstance, value);
            }

            return true;
        }
    }
}