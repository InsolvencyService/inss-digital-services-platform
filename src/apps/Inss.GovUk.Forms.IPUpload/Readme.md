# IP Upload

## Purpose

To allow IPs to upload RP14 and RP14A XML files into Dynamics and authenticate with the existing RPS database.

## Developers

To run this, you will need to do the following:

1. Edit the GovUk.Forms.HostApp launch file for the following environment variable:

```json
{
  "DOTNET_HOSTINGSTARTUPASSEMBLIES": "GovUk.Forms.Components;GovUk.Forms.HostApp;Inss.GovUk.Forms.IPUpload"
}
```

**Note** that the order is important.

This app is designed to work with mocks for external services such as Dynamics and submission service as well as an in-memory form store. 

If you want to run everything, you will need to run:

- Run the Inss.Auth.RpsProvider
- Run the Inss.Auth.Broker
- Run the GovUk.Forms.HostApp
- Run the submission service

**Notes** on getting this running with proper connectivity and loading of config, can be found in the main readme.md for the project.

You will also want to ensure you have a CosmosDb emulator running locally.

Configure the host app user secrets for the connected services:

```json
{
  "Broker": {
    "ClientId": "SHOULD MATCH BROKER VALUE",
    "IdentityProvider": "Rps",
    "JwtPublicKey": "SHARED PUBLIC KEY WITH BROKER"
  },
  "CosmosDb": {
    "ConnectionString": "AccountEndpoint=https://localhost:8081/;AccountKey=WHATEVER LOCAL EMULATOR KEY IS",
    "DatabaseName": "Forms",
    "ContainerName": "IPUpload"
  },
  "Submission": {
    "Url": "https://localhost:7134/"
  },
  "Dynamics": {
    "Url": "https://cmsuat.crm11.dynamics.com",
    "ClientId": "ASK FOR CLIENT ID",
    "ClientSecret": "ASK FOR CLIENT SECRET",
    "TenantId": "ASK FOR CLIENT SECRET"
  }
}
```

**If you are unsure** check the IaC project in GitHub. There is a section that managed the configuration for each app to help.