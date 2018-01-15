# OrchardCore.OpenId

OrchardCore.OpenId provides an implementation of an OpenID Connect server based on [OpenIddict](https://github.com/openiddict/openiddict-core) library. 
It allows Orchard Core to act as identity provider to support token authentication without the need of an external identity provider.
So, Orchard Core can be used also as an identity provider for centralizing the user access permissions to external applications not only to Orchard Core services.

Flows supported: [code/implicit/hybrid flows](http://openid.net/specs/openid-connect-core-1_0.html) and [client credentials/resource owner password grants](https://tools.ietf.org/html/rfc6749).

## Configuration

Configuration can be set through OpenID Connect settings menu in the admin dashboard and also through a recipe step.

Available settings are:

+ Testing Mode: Enabling Testing mode, removes the need of providing a certificate for signing tokens providing an ephemeral key. Also removes the requirement of using an HTTPS for issuing tokens.
+ Token Format: there are two options:
  + JWT: This format uses signed JWT standard tokens (not encrypted). It requires the SSL certificate being used is accepted as a trusted certificate by the client.
  + Encrypted: This format uses non standard opaque tokens encrypted by the ASP.NET data protection block. It doesn't require the client accept the SSL certificate as a trusted certificate.
+ Authority: Orchard url used by orchard to act as an identity server.
+ Audiences: Urls of the resource servers for which the identity server issues valid JWT tokens.
+ Certificate Store Location: CurrentUser/LocalMachine https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.storelocation(v=vs.110).aspx
+ Certificate Store Name: AddressBook/AuthRootCertificateAuthority/Disallowed/My/Root/TrustedPeople/TrustedPublisher https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.storename(v=vs.110).aspx
+ Certificate Thumbprint: The thumbprint of the certificate (It is recommended to not use same certificate it is been used for SSL).
+ Enable Token Endpoint.
+ Enable Authorization Endpoint.
+ Enable Logout Endpoint.
+ Enable User Info Endpoint.
+ Allow Password Flow: It requires Token Endpoint is enabled. More info at https://tools.ietf.org/html/rfc6749#section-1.3.3
+ Allow Client Credentials Flow: It requires Token Endpoint is enabled. More info at https://tools.ietf.org/html/rfc6749#section-1.3.4
+ Allow Authorization Code Flow: It requires Authorization and Token Endpoints are enabled. More info at http://openid.net/specs/openid-connect-core-1_0.html#CodeFlowAuth
+ Allow Implicit Flow: It requires Authorization Endpoint is enabled. More info at http://openid.net/specs/openid-connect-core-1_0.html#ImplicitFlowAuth
+ Allow Hybrid Flow: It requires Authorization and Token Endpoints. More info at http://openid.net/specs/openid-connect-core-1_0.html#HybridFlowAuth
+ Allow Refresh Token Flow: It allows to refresh access token using a refresh token. It can be used in combination with Password Flow, Authorization Code Flow and Hybrid Flow. More info at http://openid.net/specs/openid-connect-core-1_0.html#RefreshTokens

A sample of OpenID Connect Settings recipe step:
```
{
      "name": "openidsettings",
      "TestingModeEnabled": false,
      "AccessTokenFormat": "JWT", //JWT or Encrypted
      "Authority": "https://www.orchardproject.net",
      "Audiences": ["https://www.orchardproject.net","https://orchardharvest.org/"],
      "CertificateStoreLocation": "LocalMachine", //More info: https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.storelocation(v=vs.110).aspx
      "CertificateStoreName": "My", //More info: https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.storename(v=vs.110).aspx
      "CertificateThumbPrint": "27CCA66EF38EF46CD9022431FB1FF0F2DF5CA1D7"
      "EnableTokenEndpoint": true,
      "EnableAuthorizationEndpoint": false,
      "EnableLogoutEndpoint": true,
      "EnableUserInfoEndpoint": true,
      "AllowPasswordFlow": true,
      "AllowClientCredentialsFlow": false,
      "AllowAuthorizationCodeFlow": false,
      "AllowRefreshTokenFlow": false,
      "AllowImplicitFlow": false,
      "AllowHybridFlow": false
}
```

### Client OpenID Connect Apps Configuration

OpenID Connect apps can be set through OpenID Connect Apps menu in the admin dashboard and also through a recipe step.


OpenID Connect apps require the following configuration.

+ Id: Unique identifier.
+ Client Id: Client identifier of the application. It have to be provided by a client when requesting a valid token.
+ Display Name: Display name associated with the current application.
+ Type: There are two options:
  + Confidential: Confidential applications MUST send their client secret when communicating with the token and revocation endpoints. This guarantees that only the legit client can exchange an authorization code or get a refresh token.
  + Public: Public applications don't use client secret on their communications.
  + Client Secret: Client secret is a password associated with the application. It will be required when the application is configured as Confidential.
  + Flows: If general OpenID Connect settings allow this flow, app can also enable this flow.
  + Allow Password Flow: It requires Token Endpoint is enabled. More info at https://tools.ietf.org/html/rfc6749#section-1.3.3
  + Allow Client Credentials Flow: It requires Token Endpoint is enabled. More info at https://tools.ietf.org/html/rfc6749#section-1.3.4
  + Allow Authorization Code Flow: It requires Authorization and Token Endpoints are enabled. More info at http://openid.net/specs/openid-connect-core-1_0.html#CodeFlowAuth
  + Allow Implicit Flow: It requires Authorization Endpoint is enabled. More info at http://openid.net/specs/openid-connect-core-1_0.html#ImplicitFlowAuth
  + Allow Hybrid Flow: It requires Authorization and Token Endpoints. More info at http://openid.net/specs/openid-connect-core-1_0.html#HybridFlowAuth
  + Allow Refresh Token Flow: It allows to refresh access token using a refresh token. It can be used in combination with Password Flow, Authorization Code Flow and Hybrid Flow. More info at http://openid.net/specs/openid-connect-core-1_0.html#RefreshTokens
  + Normalized RoleNames: This configuration is only required inf Client Credentials Flow is enabled. It determines the roles assined to the app when it is authenticated using that flow.
  + Redirect Options: Those options are only required when Implicit Flow, Authorization Code Flow or Allow Hybrid Flow is required:
  + Logout Redirect Uri: logout callback URL
  + Redirect Uri: callback URL
  + Skip Consent: sets if a consent form has to be fulfilled by the user after log in.

  A sample of OpenID Connect App recipe step:
```
{
	  "name": "openidapplication",
      "ClientId": "openidtest",
      "DisplayName": "Open Id Test",
      "Type": "Confidential",
	  "ClientSecret": "MyPassword",
      "EnableTokenEndpoint": true,
      "EnableAuthorizationEndpoint": false,
      "EnableLogoutEndpoint": true,
      "EnableUserInfoEndpoint": true,
      "AllowPasswordFlow": true,
      "AllowClientCredentialsFlow": false,
      "AllowAuthorizationCodeFlow": false,
      "AllowRefreshTokenFlow": false,
      "AllowImplicitFlow": false,
      "AllowHybridFlow": false
}
```

## Configuring Certificates

### Windows / IIS

Several tools are available for generating a signing certificate on Windows and/or IIS, for example:

+ IIS Server Manager _(offers not much control)_
    1. Server Certificates
    2. Create Self-Signed Certificate 
+ PowerShell _(offers full control)_
    1. `New-SelfSignedCertificate`, for example:

```
# See https://technet.microsoft.com/en-us/itpro/powershell/windows/pkiclient/new-selfsignedcertificate

New-SelfSignedCertificate `
    -Subject "connect.example.com" `
    -FriendlyName "Example.com Signing Certificate" `
    -CertStoreLocation "cert:\LocalMachine\My" `
    -KeySpec Signature `
    -KeyUsage DigitalSignature `
    -KeyUsageProperty Sign `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1") `
    -KeyExportPolicy NonExportable `
    -KeyAlgorithm RSA `
    -KeyLength 4096 `
    -HashAlgorithm SHA256 `
    -NotAfter (Get-Date).AddDays(825) `
    -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider"
```

**This snippet must be run as admin.** It generates a 4096-bit signing certificate, stores it in the machine store and returns the certificate's thumbprint, which you need in the OpenID Connect Settings recipe or when exporting the certficate through PowerShell. _You should update this example according to your requirements!_

In multi-node environments consider creating the certificate with `-KeyExportPolicy Exportable`, then export the certificate (PFX) to a secure location, using the MMC Certificates Snap-In or PowerShell `Export-PfxCertificate`, and subsequently import the certificate on each node as non-exportable, which is the default when using `Import-PfxCertificate`. For example:

```
# See https://technet.microsoft.com/en-us/itpro/powershell/windows/pkiclient/export-pfxcertificate
# Run this on the machine where the certificate was generated:

$mypwd = ConvertTo-SecureString -String "MySecretPassword123" -Force -AsPlainText

Export-PfxCertificate -FilePath C:\securelocation\connect.example.com.pfx cert:\localMachine\my\thumbprintfromnewselfsignedcertificate -Password $mypwd

# See https://technet.microsoft.com/en-us/itpro/powershell/windows/pkiclient/import-pfxcertificate
# Run this on the target node:

$mypwd = ConvertTo-SecureString -String "MySecretPassword123" -Force -AsPlainText

Import-PfxCertificate -FilePath C:\securelocation\connect.example.com.pfx cert:\localMachine\my -Password $mypwd
```

**Important:** In order for the OrchardCore.OpenId module to use the certificate's keys for signing, it requires `Read` access to the certificate in the store. This can be granted in various ways, for example:

+ MMC.exe
    1. Add Snap-In 'Certificates' for Computer Account
    2. Right-Click relevant certificate and select All Tasks, Manage Private Keys
    3. Add the relevant identity (e.g. IIS AppPool\PoolName)
    4. Check Allow Read
+ WinHttpCertCfg.exe (grants Full Control)
    1. For example: `winhttpcertcfg -g -c LOCAL_MACHINE\My -s connect.example.com -a AppPoolIdentityName` https://msdn.microsoft.com/en-us/library/windows/desktop/aa384088(v=vs.85).aspx

## CREDITS

### OpenIddict

https://github.com/openiddict
License under Apache License 2.0
