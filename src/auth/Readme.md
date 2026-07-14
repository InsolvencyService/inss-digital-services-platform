# Authentication

## Broker

This is an Open ID connect compliant Identity Provider that issues ID and Access tokens for users. Users still authenticate 
with their required 3rd party identity provider e.g. Entra, One Login, RPS etc but the broker deals with the handshaking and
returns its own tokens.

It also exposes the user info endpoint. This currently requires fixing and will be used to enrich claims from our internal
data provider along with other information.

For the configuration, you will need to utilise the _user secrets_ to avoid checking in keys and secrets. The following structure
will need populating:

```json
{
  "IdentityProviders": {
    "Rps": {
      "Authority": "https://localhost:7187",
      "ClientId": "WHATEVER YOU HAVE CONFIGURED",
      "Scopes": ["openid", "email"]
    },
    "OneLogin": {
      "Authority": "https://oidc.integration.account.gov.uk",
      "ClientId": "ASK FOR CLIENT ID",
      "Scopes": ["openid", "email", "phone"],
      "JwtPrivateKey": "ASK FOR PRIVATE KEY"
    },
    "Entra": {
      "Authority": "https://login.microsoftonline.com/AS OR RETRIEVE TENANT ID FROM AZURE/v2.0",
      "ClientId": "AS OR RETRIEVE CLIENT ID FROM AZURE",
      "ClientSecret": "AS FOR CLIENT SECRET FROM AZURE",
      "Scopes": ["openid", "email"]
    }
  },
  "Broker": {
    "JwtPrivateKey": "CREATE YOUR OWN OR ASK FOR ONE",
    "TokenExpiresInMinutes": 30
  }
}

```

### Broker Public/Private Keys

The process of generating public/private keys for the broker is the same as we use for One Login.

The instructions can be found at https://docs.sign-in.service.gov.uk/before-integrating/set-up-your-public-and-private-keys/#set-up-your-public-and-private-keys

then run the two commands listed in that article and stash your keys somewhere safe and add to each project secrets.

**Try to not** check these into source control.

## RPS Identity Provider

This identity provider allows IP users to authenticate with the RPS database via a service bus relay (to be implemented) so
we do not have to force IP users across to One Login at this stage. There is work being done by a 3rd party around RPS/IPs 
which has not concluded so this solution is a stop-gap until that work has concluded.

The configuration for the provider is currently simplistic:

```json
{
  "Provider": {
    "ClientId": "WHATEVER YOUR KEY NEEDS TO BE"
  }
}
```

**Note that** as we build out the provider, the intent is to use public/private keys - but this work is outstanding.

## PKCE - Proof Key for Code Exchange

Both the RPS provider and broker use PKCE (usually pronounced pixie) to protect against CRSF attacks and authorization code 
interception/injection by issuing an authorizing code - a cryptographically signed code that is then verified by the 
authentication server. The code verifier remains with the server so if an attacker intercepts the request, all they get 
is the code and cannot exchange it for a token without the verifier.

More can be found at https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow-with-pkce