using System.ComponentModel.DataAnnotations;
using Inss.Platform.Application.Factories;
using Inss.Platform.Application.Loaders;
using Inss.Platform.Application.Validation;
using Inss.Platform.Component;
using Inss.Platform.Component.Builders;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Fip;

public sealed class FipAppBuilder : AppBuilder
{
    public override PagePath[] Build(IServiceCollection services)
    {
        PageModel page1 = PageModelBuilder
            .For("Search", "/search", new Content("Search"))
            .NextPagesAre("/search-results")
            .AddSearchTermComponent("SearchTerm", new Content("Find an insolvency practitioner"), new Content("Search"), new Content(
                "<p class=\"govuk-body\">Search using one or more of the following:</p>" +
                "<ul class=\"govuk-list govuk-list--bullet\">" +
                "<li>name</li>" +
                "<li>company</li>" +
                "<li>town or city</li>" +
                "<li>full or partial postcode</li>" +
                "<li>a combination of these</li>" +
                "</ul>"))
            .WithRequiredValidator("You must enter a search text")
            .ComponentAdded()
            .Build(services);
        
        PageModel page2 = PageModelBuilder
            .For("Last name", "/search-results", new Content("Continue"))
            .AddSingleLineTextComponent("LastName", new Content("What is your last name?"))
            .WithLoader<MyComponentLoader>()
            .WithRequiredValidator("You must supply a last name")
            .WithMaxLengthValidator(10, "Your last name is too long")
            .ComponentAdded()
            .Build(services);
        
        services.AddSingleton<IAppFactory>(_ => new AppFactory([page1, page2]));

        return [page1.Path, page2.Path];
    }
}

public sealed class MyCustomValidator : IComponentValidator
{
    public ValueTask<ValidationResult[]> ValidateAsync(ComponentModel component)
    {
        Console.WriteLine("Custom validator");
        return ValueTask.FromResult<ValidationResult[]>([]);
    }
}

public sealed class MyComponentLoader : IComponentLoader
{
    public ValueTask LoadAsync(ComponentModel component)
    {
        Console.WriteLine("Custom loader");
        return ValueTask.CompletedTask;
    }
}
