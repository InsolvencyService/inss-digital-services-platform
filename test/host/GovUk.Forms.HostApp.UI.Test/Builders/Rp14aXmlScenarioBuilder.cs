namespace GovUk.Forms.HostApp.UI.Test.Builders;

public sealed class Rp14aXmlScenarioBuilder
{
    private readonly List<Rp14aScenarioBuilder> _employees = [];

    public static Rp14aXmlScenarioBuilder Create() => new();

    public Rp14aXmlScenarioBuilder WithEmployee(
        Rp14aScenarioBuilder employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        _employees.Add(employee);

        return this;
    }

    public Rp14aXmlScenarioBuilder WithEmployees(
        params Rp14aScenarioBuilder[] employees)
    {
        ArgumentNullException.ThrowIfNull(employees);

        foreach (Rp14aScenarioBuilder employee in employees)
        {
            WithEmployee(employee);
        }

        return this;
    }

    public string BuildXml()
    {
        Rp14aBuilder builder = new();

        builder.WithNoEmployees();

        foreach (Rp14aScenarioBuilder employee in _employees)
        {
            builder.WithEmployee(
                employee.BuildEmployeeData());
        }

        return builder.Build();
    }
}
