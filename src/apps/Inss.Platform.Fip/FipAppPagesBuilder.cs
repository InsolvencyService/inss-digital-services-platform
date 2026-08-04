using System.ComponentModel.DataAnnotations;
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
        PageModel page1 = new()
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
                    Validations = [
                        new ValidationRule
                        {
                            ValidatorType = typeof(RequiredValueComponentValidator),
                            Items = new ValidationRuleItemList
                            {
                                [RequiredValueComponentValidator.ErrorMessageKey] = "You must supply a first name",
                                [RequiredValueComponentValidator.PropertyKey] = "Components[0].Value"
                            }
                        },
                        new ValidationRule
                        {
                            ValidatorType = typeof(MaxLengthComponentValidator),
                            Items = new ValidationRuleItemList
                            {
                                [MaxLengthComponentValidator.ErrorMessageKey] = "Your first name is too long",
                                [MaxLengthComponentValidator.PropertyKey] = "Components[0].Value",
                                [MaxLengthComponentValidator.MaxLengthKey] = "100"
                            }
                        }
                    ]
                }
            ],
            NextPages = ["/lastname"],
            NextPageNavigator = typeof(DefaultNextPageNavigator),
            SubmitButton = new Content("Continue")
        };
        page1.Register(services);

        PageModel page2 = new()
        {
            Title = "Page 1",
            Path = "/lastname",
            Components =
            [
                new SingleLineText
                {
                    Id = "Lastname",
                    Label = new Content(Value: "What is your last name?"),
                    Validations = [
                        new ValidationRule
                        {
                            ValidatorType = typeof(RequiredValueComponentValidator),
                            Items = new ValidationRuleItemList
                            {
                                [RequiredValueComponentValidator.ErrorMessageKey] = "You must supply a last name",
                                [RequiredValueComponentValidator.PropertyKey] = "Components[0].Value"
                            }
                        },
                        new ValidationRule
                        {
                            ValidatorType = typeof(MaxLengthComponentValidator),
                            Items = new ValidationRuleItemList
                            {
                                [MaxLengthComponentValidator.ErrorMessageKey] = "Your last name is too long",
                                [MaxLengthComponentValidator.PropertyKey] = "Components[0].Value",
                                [MaxLengthComponentValidator.MaxLengthKey] = "150"
                            }
                        }
                    ]
                }
            ],
            PreviousPage = page1.Path,
            SubmitButton = new Content("Continue")
        };
        page2.Register(services);

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
