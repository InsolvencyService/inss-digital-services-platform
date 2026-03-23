namespace GovUk.Forms.Components.Extensions;

public static class StringExtensions
{
    extension(string? value)
    {
        public string Or(string alternativeValue)
        {
            return string.IsNullOrWhiteSpace(value) ? alternativeValue : value;
        }
    }
}