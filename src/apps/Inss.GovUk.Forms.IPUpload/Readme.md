# IP Upload

## Purpose

To allow IP users to upload RP14 and RP14A XML files into Dynamics and authenticate with the existing RPS database.

## Developers

To run this, you will need to do the following:

1. Edit the GovUk.Forms.HostApp launch file for the following environment variable:

```json
{
  "DOTNET_HOSTINGSTARTUPASSEMBLIES": "GovUk.Forms.Components;Inss.GovUk.Forms.IPUpload"
}
```

**Note** that the _GovUk.Forms.Components_ **must** come first.

2. Run the Inss.Auth.RpsProvider

3. Run the Inss.Ath.Broker

4. Run the GovUk.Forms.HostApp