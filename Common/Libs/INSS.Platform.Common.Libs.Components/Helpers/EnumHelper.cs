using INSS.Platform.Common.Libs.Components.Models;
using INSS.Platform.Common.Libs.Components.Shared.Extensions;

namespace INSS.Platform.Common.Libs.Components.Helpers
{
    public static class EnumHelper
    {
        public static List<RadioOption<string>> ToRadioOptions<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum))
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