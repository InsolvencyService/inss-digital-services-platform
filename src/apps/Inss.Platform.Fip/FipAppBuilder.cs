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
            .For("First name", "/firstname", new Content("Continue"))
            .NextPagesAre("/lastname")
            .AddSingleLineTextComponent("FirstName", new Content("What is your first name?"))
            .WithLoader<MyComponentLoader>()
            .WithRequiredValidator("You must supply a first name")
            .WithMaxLengthValidator(10, "Your first name is too long")
            .ComponentAdded()
            .Build(services);
        
        PageModel page2 = PageModelBuilder
            .For("Last name", "/lastname", new Content("Continue"))
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
