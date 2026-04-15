# Gov UK Forms

## Overview

This project provides a component library with a sample _web app_ to demonstrate how to build a generic _forms_ platform for UK Government style applications.

This project levearages the NuGet package maintained by https://github.com/x-govuk/govuk-frontend-aspnetcore and builds a layer on top to provide a form-based framework.

It uses clean code architecture to separate concerns and define domain, application and infrastucture functions to the components library.

See [Clean Code Architecture](Readme.Architecture.md) for more.

## Forms

A _form_ comprises of sections with each section defining the pages of information to be collected. There is a separation of concerns whereby the way you collect data, is
abstracted from the form/section/page hierarchy itself via a flowchart process, which is detailed later.

Each page defines information you wish to collect, reusing the above library to define it's structure and a _model_ to collect the information. The _pages_ are simple _POCO_ 
classes with attributes to define any static validation.

Each form/section/page has a _Path_ property which will be unique across the whole form. For example you might have a form to collect information about you and your family 
for family credit applications so form/section/page paths might look something like:

- **Form** /family-credit
- **1st section** /family-credit/applicants-details
- **1st section page 1** /family-credit/applicants-details/your-name
- **1st section page 2** /family-credit/applicants-details/your-address
- **1st section page 3** /family-credit/applicants-details/your-dob
- **1st section summary** /family-credit/applicants-details/applicant-summary
- **2nd section** /family-credit/dependent-details
- **2nd section page 1** /family-credit/dependent-details/dependent-name
- **2nd section page 2** /family-credit/dependent-details/dependent-age
- **2nd section check answers** /family-credit/dependent-details/check-dependent-answers
- **2nd section add another (back to page 1)** /family-credit/dependent-details/add-another-dependent
- **2nd section summary /family-credit/dependent-details/dependents-summary

The above shows friendly paths which manifest as the URLs to each form/section/page and how they are formed. Each section _must_ end with a summary.

In the 2nd section it shows an example of adding a repeatable block whereby the name and dob are collected for each dependent and stored against the 
_AddAnotherModel_ _Items_ collection in the code. The decision to add another will be defined by the flowchart (described below).

The _Path_ must start with a forward slash (/) and not end with one. The form is validated upon startup to ensure that each URL is unique and 
that each page within a section has a _Path_ within the section, correctly formed.

## Form factory

This is responsible for creating an instance of a form for each user when required. It defines the data to be collected (as described above) and
on startup (when calling the _UseComponents_ extension), will have the _Validate_ extension method called to check the form is valid.

You need to implement the _IFormFactory_ interface to create your form/section/page representation for your application.

## Flowchart

See [Flowchart Framework](Readme.Flowchart.md) for more.

### Form Pre-population

If you wish to populate a form upon creation once with data that you may know due to context, such as a logged in user, you implmement the 
_IFormPrePopulation_ interface and register it with the IoC.

### Custom Page Validation

If you wish to provede more complex, custom validation for a model, such as checking the bank details entered with a 3rd party service, you can 
create an implementation inheriting from _PageValidator_ and register it with the IoC using the _keyed_ support where the _key_ is the Node Id.

Even though it might seem logical to use the _Path_, using the _Node Id_ makes it more flexible as you can use the same component but differing validation rules
such as using the date component for both dob and also date of employment. Both will have different validation rules.

## End-to-End Tests

The _GovUk.Forms.HostApp.PageTest_ project contains a few _Playwright_ tests to demonstrate usage. This also uses the _WebApplicationFactory_ in 
conjunction with _Playwright_ to allow running of the tests locally without explicitly needing to run the web app and also in a pipline.

The work is based upon the following link:

https://danieldonbavand.com/2022/06/13/using-playwright-with-the-webapplicationfactory-to-test-a-blazor-application/

which details how to get it working together nicely as it doesn't work out-of-the-box.

In a full implementation, there will be data stores and external services so these would require miocking - which is supportable for a pipline run 
as the tests run headless in that context.

Ideally we would have mocks for the externals to allow a subset of the tests to run at build time and then also a fuller pack of tests 
that can run against and environment after deployment - without mocks.

To get these running locally, from the repository root:

```bash
cd test/host/GovUk.Forms.HostApp.PageTest

bin/Debug/net10.0/playwright.ps1 install

```

## Links

https://github.com/x-govuk/govuk-frontend-aspnetcore

https://design-system.dwp.gov.uk/patterns/add-another-thing/examples/user-journey

https://dwp-design-system-examples-c0557c2c3d1d.herokuapp.com/patterns/add-another-thing/v1

https://design-system.service.gov.uk/patterns/check-answers/

https://design.tax.service.gov.uk/hmrc-design-patterns/add-to-a-list/

https://service-manual.ons.gov.uk/design-system/patterns/error-status-pages

https://design-system.service.gov.uk/patterns/problem-with-the-service-pages/

https://digital-services-prototypes.azurewebsites.net - prototype

## Other

If using a Mac, install homebrew to enable installation of the playywright tooling:

```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

Then install powershell suppprt:

```bash
brew install --cask powershell
```

Finally, navigate to /test/GovUk.Forms.WebApp.Test in a terminal and run:

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

## Useful

https://www.youtube.com/watch?v=NXMfN-2ekAM&t=31s


