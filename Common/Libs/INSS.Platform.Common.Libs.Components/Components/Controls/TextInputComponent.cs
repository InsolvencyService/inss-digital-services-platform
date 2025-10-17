using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace INSS.Platform.Common.Libs.Components.Components.Controls
{
    /// <summary>
    /// An input component for editing <see cref="string"/> values.
    /// </summary>
    public class TextInputComponent : InputBase<string?>
    {
        /// <summary>
        /// Gets or sets the associated <see cref="ElementReference"/>.
        /// <para>
        /// May be <see langword="null"/> if accessed before the component is rendered.
        /// </para>
        /// </summary>
        [DisallowNull] public ElementReference? Element { get; protected set; }

        // New parameters for label support
        [Parameter] public string? Label { get; set; }

        // Generated fallback id for input when none is provided via AdditionalAttributes
        private string _generatedId = string.Empty;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            // Create a stable id for this component instance
            _generatedId = $"textinput_{Guid.NewGuid():N}";
        }

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int seq = 0;

            // Determine id to use: prefer AdditionalAttributes["id"] if present
            string idToUse = (AdditionalAttributes is not null
                && AdditionalAttributes.TryGetValue("id", out object? idObj)
                && idObj is string idStr
                && !string.IsNullOrEmpty(idStr))
                ? idStr
                : _generatedId;

            // Start rendering the form group
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "govuk-form-group");

            // Render label if provided
            if (!string.IsNullOrEmpty(Label))
            {
                builder.OpenElement(seq++, "h1");
                builder.AddAttribute(seq++, "class", "govuk-label-wrapper");
                builder.OpenElement(seq++, "label");
                builder.AddAttribute(seq++, "class", "govuk-label govuk-label--l");
                builder.AddAttribute(seq++, "for", idToUse);
                builder.AddContent(seq++, Label);
                builder.CloseElement();
                builder.CloseElement();
            }

            // Render input
            builder.OpenElement(seq++, "input");
            builder.AddMultipleAttributes(seq++, AdditionalAttributes);
            if (!string.IsNullOrEmpty(NameAttributeValue))
            {
                builder.AddAttribute(seq++, "name", NameAttributeValue);
            }
            builder.AddAttribute(seq++, "class", "govuk-input");

            // Ensure an id attribute is present so the label can reference it
            builder.AddAttribute(seq++, "id", idToUse);

            builder.AddAttribute(seq++, "value", CurrentValueAsString);
            builder.AddAttribute(seq++, "onchange", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.SetUpdatesAttributeName("value");
            builder.AddElementReferenceCapture(seq++, __inputReference => Element = __inputReference);
            builder.CloseElement();

            // Close form group
            builder.CloseElement();
        }

        /// <inheritdoc />
        protected override bool TryParseValueFromString(string? value, out string? result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            result = value;
            validationErrorMessage = null;
            return true;
        }
    }
}