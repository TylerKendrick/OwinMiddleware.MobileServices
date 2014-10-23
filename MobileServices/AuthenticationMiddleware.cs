using System;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace MobileServices
{
    public static partial class WebApi
    {
        public abstract class AuthenticationMiddleware : AuthenticationMiddleware<AuthenticationOptions>
        {
            private readonly HttpClient _httpClient;

            protected AuthenticationMiddleware(OwinMiddleware next,
                IAppBuilder app, AuthenticationOptions options, string middlewareName)
                : base(next, options)
            {
                if (String.IsNullOrEmpty(Options.SignInAsAuthenticationType))
                    Options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();

                if (Options.StateDataFormat == null)
                {
                    var dataProtector = app.CreateDataProtector(
                        middlewareName, Options.AuthenticationType, "v1");
                    Options.StateDataFormat = new PropertiesDataFormat(dataProtector);
                }
                _httpClient = new HttpClient(ResolveHttpMessageHandler(Options))
                {
                    Timeout = Options.BackchannelTimeout,
                    MaxResponseContentBufferSize = 1024*1024*10,
                };
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft Owin SmartThings middleware");
                _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            }

            protected override AuthenticationHandler<AuthenticationOptions> CreateHandler()
            {
                return CreateHandler(_httpClient);
            }

            protected abstract AuthenticationHandler<AuthenticationOptions> CreateHandler(HttpClient httpClient);

            private static HttpMessageHandler ResolveHttpMessageHandler(AuthenticationOptions options)
            {
                var handler = options.BackchannelHttpHandler ?? new WebRequestHandler();

                // If they provided a validator, apply it or fail.
                if (options.BackchannelCertificateValidator != null)
                {
                    // Set the cert validate callback
                    var webRequestHandler = handler as WebRequestHandler;
                    if (webRequestHandler == null)
                    {
                        throw new InvalidOperationException();
                    }
                    webRequestHandler.ServerCertificateValidationCallback =
                        options.BackchannelCertificateValidator.Validate;
                }

                return handler;
            }
        }
    }
}