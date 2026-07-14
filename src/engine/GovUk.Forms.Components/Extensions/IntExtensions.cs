using System.Globalization;

namespace GovUk.Forms.Components.Extensions;

public static class IntExtensions
{
    extension(int value)
    {
        public string AsString()
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        
        public string AsMoney()
        {
            return value.ToString("C", CultureInfo.CurrentCulture);
        }
    }
}