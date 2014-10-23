using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace MobileServices
{
    public static partial class WebApi
    {
        public class AuthenticationOptions : Microsoft.Owin.Security.AuthenticationOptions, IAuthenticationOptions
        {
            public Uri AuthEndpoint { get; set; }
            public Uri TokenEndpoint { get; set; }
            public PathString CallbackPath { get; set; }
            public ICertificateValidator BackchannelCertificateValidator { get; set; }
            public HttpMessageHandler BackchannelHttpHandler { get; set; }
            public TimeSpan BackchannelTimeout { get; set; }

            public string Caption
            {
                get { return Description.Caption; }
                set { Description.Caption = value; }
            }

            public IList<string> Scope { get; private set; }
            public string SignInAsAuthenticationType { get; set; }
            public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
            public string DefaultIssuer { get; set; }

            public AuthenticationOptions(string providerName, string defaultIssuer,
                Uri tokenEndpoint, Uri authEndpoint)
                : base(providerName)
            {
                Caption = providerName;
                DefaultIssuer = defaultIssuer;
                TokenEndpoint = tokenEndpoint;
                AuthEndpoint = authEndpoint;
                CallbackPath = new PathString("/signin-" + providerName);
                AuthenticationMode = AuthenticationMode.Passive;
                Scope = new List<string>();
                BackchannelTimeout = TimeSpan.FromSeconds(60);
            }
        }
    }
}