
# Run this from the repository root
cd C:\Dev\inss-digital-services-platform

# Change dir to project to run
cd src\auth\Inss.Auth.Broker

# Build and publish it as production configuration
dotnet publish --configuration Release

# Change dir to publish location
cd bin\Release\net10.0\publish

# Config env vars
$env:DOTNET_HOSTINGSTARTUPASSEMBLIES="GovUk.Forms.Components;Inss.Auth.Broker"
$env:APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=your-key-here"

# Run it as production
dotnet Inss.Auth.Broker.dll --environment ASPNETCORE_ENVIRONMENT=Production --config "PATH TO YOUR APPSETTINGS OR SECRETS JSON" --urls "https://localhost:7073"