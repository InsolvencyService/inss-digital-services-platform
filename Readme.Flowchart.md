# Flowchart Framework

As the form defines sections and pages only, the way that you define how to populate the pages in a section, is defined via a flowchart.

To define a section flowchart, you need to inherit the _DefineFlowchartBuilder_ abstract class in your app:

```c#
public sealed class YourDetailsFlowchart : DefineFlowchartBuilder
{
    public override void Construct(IServiceCollection services)
    {
        NodeId fullNameId = NodeId.New();
        NodeId addressId = NodeId.New();
        NodeId ageId = NodeId.New();
        NodeId salaryId = NodeId.New();
        NodeId bankAccountId = NodeId.New();
        NodeId ownHomeId = NodeId.New();
        NodeId homeValueId = NodeId.New();
        NodeId summaryId = NodeId.New();
        WebRoot webRoot = new();
        
        FormModel form = GetForm(services, webRoot.Root);
        SectionModel section = form.Sections["Your Details"];
        
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        SalaryModel salary = section.Pages.GetFirstOf<SalaryModel>();
        BankAccountModel bankAccount = section.Pages.GetFirstOf<BankAccountModel>();
        OwnHomeModel ownHome = section.Pages.GetFirstOf<OwnHomeModel>();
        HomeValueModel homeValue = section.Pages.GetFirstOf<HomeValueModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        
        FlowchartBuilder
            .ForSection(section, services)
            .AddTransitionNode(fullNameId, fullName.Path, addressId)
            .Next()
            .AddTransitionNode(addressId, address.Path, ageId)
            .Next()
            .AddDecisionNode(ageId, age.Path, salaryId, summaryId)
            .WithExecutor<YourAgeFlowNodeExecutor>()
            .Next()
            .AddDecisionNode(salaryId, salary.Path, bankAccountId, summaryId)
            .WithExecutor<YourSalaryFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(bankAccountId, bankAccount.Path, ownHomeId)
            .WithValidator<BankAccountFlowNodeValidator>()
            .Next()
            .AddDecisionNode(ownHomeId, ownHome.Path, homeValueId, summaryId)
            .WithExecutor<OwnHomeFlowNodeExecutor>()
            .Next()
            .AddTransitionNode(homeValueId, homeValue.Path, summaryId)
            .Next()
            .AddEndNode(summaryId, summary.Path)
            .WithLoader<SectionSummaryFlowNodeLoader>()
            .WithExecutor<SectionSummaryFlowNodeExecutor>()
            .BuildAndRegister();
    }
}
```

Each _node_ in the flowchart needs to have a unique Id - this is how we link nodes together. The page path should not be used as it is
possible that different flows through the flowchart may link to the same page.

As the use progresses through the section answering questions, the next node and associated page are linked via the _LinkedToNode_ page property.

There is a helper to get an instance of the _form_ so you have access the section you wish to create the flowchart for.

Get each page ready to include in the building process then use the _FlowchartBuilder_ to add each node with desired behaviours.

You always finish with an end node. The _BuildAndRegister_ performs validation to check everything is setup correctly.

Different _nodes_ allow for custom loaders, validators and executors to be defined by attaching them to the node. This means that a specific
page which appears in multiple places, can have different validation - for instance, dependent on context. 

Upon startup, the app will automatically register any flowcharts implementing the above abstract class. 

## Node Extensions

Below is a list of all the available node extensions and usage:

- **IFlowNodeLoader** via WithLoader - allows you to define some modification before the page is displayed to the user such 
as pre-population of data and also allows for altering the page to load if required, such as when add another has no items.
- **IFlowNodeValidator** via WithValidator - allows for custom validation before a page is saved such as verifying bank 
details with 3rd parties.
- **IFlowNodeExecutor** via WithExecutor - allows for processing a page when it is posted and will determine the next node
to navigate to, such when processing the add another thing.

## Add Another Thing

This pattern works by allowing you to add multiple items, in series and repeat. It uses _working pages_ which collect the
data and then present a _check_ answers_, allowing for editing before being sent to the _add another_ page. The extension
_AddAnotherWorkingPageFlowNodeExecutor_ is responsible for updating the collection of added items, as each working page 
is saved - by adding or updating the _items_ array on the _AddAnotherModel_.

Once on the _add another_ page, the _AddAnotherFlowNodeExecutor_ decides if we want to add another item by checking the state
of the _AddAnotherItem_ flag and, if set, it will clear the _working pages_ to prepare it before setting the next node to 
return to. The next node will either be to continue onto the next page or return to the first working page.

## Validating Pages

The _IFlowNodeValidator_ provides an extension to validate a page before it is saved. The default behaviour is to validate 
against model attributes but can use custom validation as required.

Such examples might be calling a third party to validate bank details that have passed a general format validation but we
want to ensure that they actually exist.

## Saving Pages

The _IFlowNodeExecutor_ handles processing the state of the page before it is saved to the form and persisted. It is responsible 
for determining the next node to go to for a page, setting the section state on the summary or handling the removal of an
item from the add another then.