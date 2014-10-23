using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace MobileServiceProviders
{
    public interface IAuthenticationOptions
    {
        Uri ApiEndpoint { get; set; }
        PathString TokenPath { get; set; }
        PathString AuthorizationPath { get; set; }
        PathString CallbackPath { get; set; }
        ICertificateValidator BackchannelCertificateValidator { get; set; }
        HttpMessageHandler BackchannelHttpHandler { get; set; }
        TimeSpan BackchannelTimeout { get; set; }
        string Caption { get; set; }
        IList<string> Scope { get; }
        string SignInAsAuthenticationType { get; set; }
        ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
        string DefaultIssuer { get; set; }
        string AuthenticationType { get; set; }
        AuthenticationMode AuthenticationMode { get; set; }
        AuthenticationDescription Description { get; set; }
    }
}