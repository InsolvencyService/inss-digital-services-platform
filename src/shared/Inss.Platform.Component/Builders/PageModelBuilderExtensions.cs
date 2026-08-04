using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Component.Builders;

public static class PageModelBuilderExtensions
{
    extension(PageModelBuilder pageModelBuilder)
    {
        public ComponentModelBuilder AddSingleLineTextComponent(ComponentId id, Content label)
        {
            SingleLineText component = new() { Id = id, Label = label };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
    }
}