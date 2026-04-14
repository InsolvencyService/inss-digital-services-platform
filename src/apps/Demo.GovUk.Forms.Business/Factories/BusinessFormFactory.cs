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
            .AddGroupPage<FullNameModel>("Employee name", "employee-name", question: "What is the employee's name?", hint: "Enter their first and last name", submitButtonText: "Continue")
            .AddGroupPage<AgeModel>("Employee age", "employee-age", question: "What is the employee's age?", submitButtonText: "Continue")
            .AddGroupPage<CheckAnswersModel>("Check employee details", "check-employee-details", submitButtonText: "Continue")
            .AddGroupPage<RemoveModel>("Remove employee details", "remove-employee-details", submitButtonText: "Continue")
            .AddFinalGroupPage<AddAnotherModel>("Employee details", "add-another-employee", submitButtonText: "Continue")
            .EndSection<SummaryModel>("Employee summary", "summary", submitButtonText: "Continue")
            
            .AddSection("Creditors and Debtors", "your-creditors-and-debtors")
            .AddGroup<AddAnotherGroup>("Creditors")
            .AddGroupPage<FullNameModel>("Creditor name", "creditor-name", question: "What is the creditor's name?", hint: "Enter their first and last name", submitButtonText: "Continue")
            .AddGroupPage<MoneyModel>("Amount owned to creditor", "amount-owned-to-creditor", question: "What is the amount owned to the creditor?", submitButtonText: "Continue")
            .AddGroupPage<CheckAnswersModel>("Check creditor details", "check-creditor-details", submitButtonText: "Continue")
            .AddGroupPage<RemoveModel>("Remove creditor details", "remove-creditor-details", submitButtonText: "Continue")
            .AddFinalGroupPage<AddAnotherModel>("Creditor details", "add-another-creditor", submitButtonText: "Continue")
            .AddGroup<AddAnotherGroup>("Debtors")
            .AddGroupPage<FullNameModel>("Debtor name", "debtor-name", question: "What is the debtor's name?", hint: "Enter their first and last name", submitButtonText: "Continue")
            .AddGroupPage<MoneyModel>("Amount owned by the debtor", "amount-owned-by-debtor", question: "What is the amount owned by the debtor?", submitButtonText: "Continue")
            .AddGroupPage<CheckAnswersModel>("Check debtor details", "check-debtor-details", submitButtonText: "Continue")
            .AddGroupPage<RemoveModel>("Remove debtor details", "remove-debtor-details", submitButtonText: "Continue")
            .AddFinalGroupPage<AddAnotherModel>("Debtor details", "add-another-debtor", submitButtonText: "Continue")
            .EndSection<SummaryModel>("Creditors and debtors summary", "summary", submitButtonText: "Continue")
            
            .ValidateAndComplete();
    }
}