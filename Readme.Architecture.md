# Clean Code Architecture

Below is a basic diagram of the components that form the architecture of the project:

![image](doc/Clean%20Code%20Architecture.png)

The following sections discuss each in more detail.

## Domain

The _domain_ layer is where we define our _models_ and supporting rules and helpers. This has no dependencies on other projects
but will be consumed by the application layer and, indirectly the components (presentation layer).

This project also has some specific functionality to handle serialization of the domain, enums that are used and _primitives_ to
ensure domain _identifiers_ are not mixed up.

## Application

This layer provides the core functionality for the application. This is where the flowchart behaviour is defined, the extension points 
that allow the components or web app to extend such as the form factory for creating instances of the form or the processing or decision 
making of the flowchart.

## Infrastructure

This layer provides out external implementation such as where to store and retrieve the form for a user, the providing of user identity 
and, as required, external calls to databases or 3rd party services such as bank validation or other internal APIs.

**Its is important** to note that the presentation layer _only_ bootstraps this layer via the provided service extension. All other operartions
are via indirect calls through the _application_ layer via interfaces and abstractions.

## Component

This layer is part of the presentation layer and is what allows other projects, such as the demo web app to consume the components and build 
working forms. Itself provides web app build and web app extensions and will resolve the form via thwe factory, upon startup and configure the 
routing for the whole form, validate the form and also the flowchart for each section.

## Host App

This is a host project which comsumes the components and provides an some demo forms with multiple sections to demonstrate usage and test functionality.

It uses the web hosting patter. See [Clean Code Architecture](Readme.Hosting.md) for more details.

## Links

[Clean Code Architecture](doc/Clean%20Code%20Architecture.drawio)