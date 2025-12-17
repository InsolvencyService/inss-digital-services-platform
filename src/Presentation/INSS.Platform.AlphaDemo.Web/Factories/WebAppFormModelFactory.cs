using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.AlphaDemo.Web.Factories;

/// <summary>
/// Provides a factory for creating and initializing instances of <see cref="FormModel"/> for the web application.
/// </summary>
/// <remarks>
/// This implementation creates a new <see cref="FormModel"/> and invokes <see cref="FormModel.Initialize"/> to ensure
/// the model is in a valid, ready-to-use state for Razor Pages.
/// </remarks>
public sealed class WebAppFormModelFactory : IFormModelFactory
{
    /// <summary>
    /// Asynchronously creates a new, initialized instance of <see cref="FormModel"/>.
    /// </summary>
    /// <returns>
    /// A task that, when completed, contains the initialized <see cref="FormModel"/>.
    /// </returns>
    /// <remarks>
    /// The returned task is completed synchronously using <see cref="Task.FromResult{TResult}(TResult)"/>.
    /// </remarks>
    public Task<FormModel> CreateAsync()
    {
        FormModel form = new()
        {
            Title = "Demo task list",
            Sections =
            [
                new SectionModel
                {
                    Title = "About you",
                    PathName = "about-you",
                    Pages =
                    [
                        new FullNameModel(),
                        new DateOfBirthModel(),
                        new AddressModel(),
                        new TelephoneNumberModel(),
                        new EmailAddressModel()
                    ]
                },
                new SectionModel
                {
                    Title = "Income",
                    PathName = "income",
                    Pages =
                    [
                        new SourceOfIncomeModel(),
                        new GrossIncomeModel(),
                        new PaymentFrequencyModel(),
                        new IncomeProviderModel(),
                        new AddAnotherModel
                        { 
                            Items = 
                            [
                                [
                                    new SourceOfIncomeModel(), 
                                    new GrossIncomeModel(), 
                                    new PaymentFrequencyModel(), 
                                    new IncomeProviderModel()
                                ]
                            ] 
                        }
                    ]
                },
                new SectionModel
                {
                    Title = "Bank validation",
                    PathName = "bank-validation",
                    Pages =
                    [
                        new BankAccountModel(),
                    ]
                }
            ]
        };

        form.Initialize();

        return Task.FromResult(form);
    }
}