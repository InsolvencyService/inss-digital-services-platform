using INSS.Platform.Components.Web.Models;
using INSS.Platform.Components.Web.Common.Extensions;

namespace INSS.Platform.Components.Web.Helpers
{
    public static class EnumHelper
    {
        public static List<RadioOption<string>> ToRadioOptions<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>()
                .Cast<TEnum>()
                .Select(e => new RadioOption<string>
                {
                    Value = e.ToString(),
                    Text = e.GetDescription(),
                })
                .ToList();
        }
    }
}