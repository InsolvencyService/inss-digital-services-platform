using INSS.Platform.Forms.Web.TestHarness.Common.Services;
using INSS.Platform.Forms.Web.TestHarness.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace INSS.Platform.Forms.Web.TestHarness.Common.Validators
{
    public static class FormValidationHelper
    {
        public static bool ValidateAndAssignFormProperty(
            IPropertyValidator propertyValidator,
            EditContext editContext,
            AboutYou aboutYouInstance,
            AboutYou formToValidate,
            AboutYou formToPersist,
            string property)
        {
            object? value = formToValidate.GetType().GetProperty(property)?.GetValue(formToValidate);

            IList<System.ComponentModel.DataAnnotations.ValidationResult> errors = propertyValidator.ValidateProperties(formToValidate, new[] { property });
            if (errors.Any())
            {
                ValidationMessageStore validationMessageStore = new ValidationMessageStore(editContext);
                foreach (ValidationResult error in errors)
                {
                    FieldIdentifier fieldIdentifier = new FieldIdentifier(aboutYouInstance, error.MemberNames.First());
                    validationMessageStore.Add(fieldIdentifier, error.ErrorMessage!);
                }

                editContext.NotifyValidationStateChanged();
                SetPropertyValueByName(aboutYouInstance, property, value);
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

        public static bool ValidateAndAssignFormProperty<TComplex>(
            IPropertyValidator propertyValidator,
            EditContext editContext,
            AboutYou aboutYouInstance,
            TComplex complexPropertyToValidate,
            AboutYou formToPersist,
            string propertyName)
        {
            IList<ValidationResult> errors = propertyValidator.ValidateAllProperties(complexPropertyToValidate);
            if (errors.Any())
            {
                ValidationMessageStore validationMessageStore = new(editContext);

                object? formInstance = typeof(AboutYou).GetProperty(propertyName)?.GetValue(aboutYouInstance);
                if (formInstance is not { })
                {
                    throw new InvalidOperationException("The property to validate must exist on the bound instance of the Form property.");
                }

                foreach (ValidationResult error in errors)
                {
                    foreach (string memberName in error.MemberNames)
                    {
                        FieldIdentifier fieldIdentifier = new(formInstance, memberName);
                        validationMessageStore.Add(fieldIdentifier, error.ErrorMessage!);
                    }
                }

                editContext.NotifyValidationStateChanged();

                foreach (PropertyInfo prop in typeof(TComplex).GetProperties())
                {
                    object? value = prop.GetValue(complexPropertyToValidate);
                    prop.SetValue(formInstance, value);
                }

                return false;
            }

            object? targetInstance = typeof(AboutYou).GetProperty(propertyName)?.GetValue(formToPersist);
            foreach (PropertyInfo prop in typeof(TComplex).GetProperties())
            {
                object? value = prop.GetValue(complexPropertyToValidate);
                prop.SetValue(targetInstance, value);
            }

            return true;
        }
    }
}