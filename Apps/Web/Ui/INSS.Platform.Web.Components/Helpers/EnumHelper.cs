using INSS.Platform.Web.Components.Models;
using INSS.Platform.Web.Components.Common.Extensions;

namespace INSS.Platform.Web.Components.Helpers
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