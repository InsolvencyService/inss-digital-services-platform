using GovUk.Forms.Components.Builders;
using GovUk.Forms.Components.Factories;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.Business.Factories;

public sealed class BusinessFormFactory : IFormFactory
{
    public FormModel Create()
    {
        return FormModelBuilder
            .Create("business")
            
            .AddSection("Employee Details", "your-employee-details")
            .AddGroup<AddAnotherGroup>("Employees")
            .AddGroupPage<FullNameModel>("Employee name", "employee-name", question: "What is the employee's name?", hint: "Enter their first and last name")
            .AddGroupPage<AgeModel>("Employee age", "employee-age", question: "What is the employee's age?")
            .AddGroupPage<CheckAnswersModel>("Check employee details", "check-employee-details")
            .AddGroupPage<RemoveModel>("Remove employee details", "remove-employee-details")
            .AddFinalGroupPage<AddAnotherModel>("Employee details", "add-another-employee")
            .AddSummary("Employee summary", "summary")
            
            .AddSection("Creditors and Debtors", "your-creditors-and-debtors")
            .AddGroup<AddAnotherGroup>("Creditors")
            .AddGroupPage<FullNameModel>("Creditor name", "creditor-name", question: "What is the creditor's name?", hint: "Enter their first and last name")
            .AddGroupPage<MoneyModel>("Amount owned to creditor", "amount-owned-to-creditor", question: "What is the amount owned to the creditor?")
            .AddGroupPage<CheckAnswersModel>("Check creditor details", "check-creditor-details")
            .AddGroupPage<RemoveModel>("Remove creditor details", "remove-creditor-details")
            .AddFinalGroupPage<AddAnotherModel>("Creditor details", "add-another-creditor")
            .AddGroup<AddAnotherGroup>("Debtors")
            .AddGroupPage<FullNameModel>("Debtor name", "debtor-name", question: "What is the debtor's name?", hint: "Enter their first and last name")
            .AddGroupPage<MoneyModel>("Amount owned by the debtor", "amount-owned-by-debtor", question: "What is the amount owned by the debtor?")
            .AddGroupPage<CheckAnswersModel>("Check debtor details", "check-debtor-details")
            .AddGroupPage<RemoveModel>("Remove debtor details", "remove-debtor-details")
            .AddFinalGroupPage<AddAnotherModel>("Debtor details", "add-another-debtor")
            .AddSummary("Creditors and debtors summary", "summary")
            
            .ValidateAndComplete();
    }
}