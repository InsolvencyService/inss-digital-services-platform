using Inss.Platform.Domain.Components.Common;
using Inss.Platform.Domain.Components.Searching;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Component.Builders;

public static class PageModelBuilderExtensions
{
    extension(PageModelBuilder pageModelBuilder)
    {
        public ComponentModelBuilder AddSingleLineTextComponent(ComponentId id, string question)
        {
            SingleLineTextComponentModel component = new()
            {
                Id = id, AssociatedPagePath = pageModelBuilder.CurrentPage.Path, Question = question
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }

        public ComponentModelBuilder AddSearchTermComponent(ComponentId id, string heading, string label, string description)
        {
            SearchTermComponentModel component = new()
            {
                Id = id, AssociatedPagePath = pageModelBuilder.CurrentPage.Path, Heading = heading, Label = label, Description = description
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
        
        public ComponentModelBuilder AddSearchResultComponent(ComponentId id, string label, string configKey, PagePath resultDetailPath)
        {
            SearchResultComponentModel component = new()
            {
                Id = id, AssociatedPagePath = pageModelBuilder.CurrentPage.Path, Label = label, ConfigKey = configKey, ResultDetailPath = resultDetailPath
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
        
        public ComponentModelBuilder AddSearchResultDetailComponent(ComponentId id, string configKey)
        {
            SearchResultDetailComponentModel component = new()
            {
                Id = id, AssociatedPagePath = pageModelBuilder.CurrentPage.Path, ConfigKey = configKey
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
    }
}