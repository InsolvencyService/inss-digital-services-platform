using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Components.Common;
using Inss.Platform.Domain.Components.Searching;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Component.Builders;

public static class PageModelBuilderExtensions
{
    extension(PageModelBuilder pageModelBuilder)
    {
        public ComponentModelBuilder AddSingleLineTextComponent(string question, string? hint = null)
        {
            SingleLineTextComponentModel component = new()
            {
                Id = pageModelBuilder.GetNextComponentId(), 
                AssociatedPagePath = pageModelBuilder.CurrentPage.Path, 
                Question = question,
                Hint = hint
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
        
        public ComponentModelBuilder AddEmailComponent(string question, string? hint = null)
        {
            EmailComponentModel component = new()
            {
                Id = pageModelBuilder.GetNextComponentId(), 
                AssociatedPagePath = pageModelBuilder.CurrentPage.Path, 
                Question = question,
                Hint = hint
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
        
        public ComponentModelBuilder AddPasswordComponent(string question)
        {
            PasswordComponentModel component = new()
            {
                Id = pageModelBuilder.GetNextComponentId(), 
                AssociatedPagePath = pageModelBuilder.CurrentPage.Path, 
                Question = question
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
        
        public ComponentModelBuilder AddHeadingComponent(string heading)
        {
            HeadingComponentModel component = new()
            {
                Id = pageModelBuilder.GetNextComponentId(), 
                AssociatedPagePath = pageModelBuilder.CurrentPage.Path, 
                Heading = heading,
                ComponentType = ComponentTypes.BeforeForm // TODO: Perhaps pass in?
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }

        public ComponentModelBuilder AddLinkComponent(string label, string url)
        {
            LinkComponentModel component = new()
            {
                Id = pageModelBuilder.GetNextComponentId(), 
                AssociatedPagePath = pageModelBuilder.CurrentPage.Path, 
                Label = label,
                Url = url,
                ComponentType = ComponentTypes.AfterForm // TODO: Perhaps pass in?
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
        
        public ComponentModelBuilder AddSearchTermComponent(string heading, string label, string description)
        {
            SearchTermComponentModel component = new()
            {
                Id = pageModelBuilder.GetNextComponentId(), 
                AssociatedPagePath = pageModelBuilder.CurrentPage.Path,
                Heading = heading, 
                Label = label, 
                Description = description
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
        
        public ComponentModelBuilder AddSearchResultComponent(string label, string configKey, PagePath resultDetailPath)
        {
            SearchResultComponentModel component = new()
            {
                Id = pageModelBuilder.GetNextComponentId(), 
                AssociatedPagePath = pageModelBuilder.CurrentPage.Path, 
                Label = label, 
                ConfigKey = configKey, 
                ResultDetailPath = resultDetailPath
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
        
        public ComponentModelBuilder AddSearchResultDetailComponent(string configKey)
        {
            SearchResultDetailComponentModel component = new()
            {
                Id = pageModelBuilder.GetNextComponentId(), 
                AssociatedPagePath = pageModelBuilder.CurrentPage.Path, 
                ConfigKey = configKey
            };
            pageModelBuilder.CurrentPage.Components = [..pageModelBuilder.CurrentPage.Components, component];
            return new ComponentModelBuilder(pageModelBuilder, component);
        }
    }
}