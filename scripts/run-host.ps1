
# Run this from the repository root
cd C:\Dev\inss-digital-services-platform

# Change dir to project to run
cd src\host\GovUk.Forms.HostApp

# Build and publish it as production configuration
dotnet publish --configuration Release

# Change dir to publish location
cd bin\Release\net10.0\publish

# Config env vars
$env:DOTNET_HOSTINGSTARTUPASSEMBLIES="GovUk.Forms.Components;GovUk.Forms.HostApp;Inss.GovUk.Forms.IPUpload"
$env:APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=your-key-here"

# Run it as production
dotnet GovUk.Forms.HostApp.dll --environment ASPNETCORE_ENVIRONMENT=Production --config "PATH TO YOUR APPSETTINGS OR SECRETS JSON" --urls "https://localhost:5056"