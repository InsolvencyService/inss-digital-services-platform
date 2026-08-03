using Inss.Platform.Application.Factories;
using Inss.Platform.Application.Loaders;
using Inss.Platform.Application.Navigators;
using Inss.Platform.Application.Validation;
using Inss.Platform.Component;
using Inss.Platform.Component.Extensions;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Loading;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.Domain.Validation;

namespace Inss.Platform.Fip;

public sealed class FipAppPagesBuilder : AppPagesBuilder
{
    public override PagePath[] Build(IServiceCollection services)
    {
        Page page1 = new Page
        {
            Title = "Page 1",
            Path = "/firstname",
            Components =
            [
                new SingleLineText
                {
                    Id = "Fullname",
                    Label = new Content(Value: "What is your first name?"),
                    Loaders = [
                        new Loader { LoaderType = typeof(MyComponentLoader) }
                    ],
                    Validations =
                    [
                        new Required(),
                        new MaxLength { Value = 100 },
                        new Custom { ValidatorType = typeof(MyCustomValidator) }
                    ]
                }
            ],
            NextPages = ["/lastname"],
            NextPageNavigator = typeof(DefaultNextPageNavigator)
        };
        page1.Register(services);

        Page page2 = new Page
        {
            Title = "Page 1",
            Path = "/lastname",
            Components =
            [
                new SingleLineText
                {
                    Id = "Lastname",
                    Label = new Content(Value: "What is your last name?"),
                    Validations =
                    [
                        new Required(),
                        new MaxLength { Value = 150 }
                    ]
                }
            ],
            PreviousPage = page1.Path
        };
        page2.Register(services);

        services.AddSingleton<IAppFactory>(_ => new AppFactory([page1, page2]));

        return [page1.Path, page2.Path];
    }
}

public sealed class MyCustomValidator : IComponentValidator
{
    public ValueTask ValidateAsync(Inss.Platform.Domain.Components.Component component)
    {
        Console.WriteLine("Custom validator");
        return ValueTask.CompletedTask;
    }
}

public sealed class MyComponentLoader : IComponentLoader
{
    public ValueTask LoadAsync(Inss.Platform.Domain.Components.Component component)
    {
        Console.WriteLine("Custom loader");
        return ValueTask.CompletedTask;
    }
}
