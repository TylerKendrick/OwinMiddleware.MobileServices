using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using MobileServiceProviders;
using Newtonsoft.Json;
using Owin;

namespace MobileServices
{
    public class SmartThings
    {
        private static readonly Uri AuthEndpoint = new Uri("https://graph.api.smartthings.com/oauth/authorize");
        private static readonly Uri TokenEndpoint = new Uri("https://graph.api.smartthings.com/oauth/token");
        public const string ProviderName = "SmartThings";
        private const string ClientId = "Client Application ID Goes Here";
        private const string ClientSecret = "Client Secret Key Goes Here";
        private const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";

        public class EndpointBuilder : WebApi.EndpointBuilder
        {
            public EndpointBuilder()
                : base(AuthEndpoint, TokenEndpoint)
            {
            }

            protected override IEnumerable<KeyValuePair<string, string>> TokenParameters(string state,
                string code, string redirectUri)
            {
                return new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"client_id", ClientId},
                    {"client_secret", ClientSecret},
                    {"redirect_uri", redirectUri},
                    {"state", state},
                    {"code", code}
                };
            }

            protected override IEnumerable<KeyValuePair<string, string>> AuthorizationParameters(string state,
                string scope, string redirectUri)
            {
                return new Dictionary<string, string>
                {
                    {"client_id", ClientId},
                    {"state", state},
                    {"scope", scope},
                    {"response_type", "code"},
                    {"redirect_uri", redirectUri}
                };
            }
        }

        public class AuthenticationMiddleware : WebApi.AuthenticationMiddleware
        {
            private readonly ILogger _logger;

            public AuthenticationMiddleware(OwinMiddleware next, IAppBuilder app,
                WebApi.AuthenticationOptions options)
                : base(next, app, options, typeof(AuthenticationMiddleware).FullName)
            {
                _logger = app.CreateLogger<AuthenticationMiddleware>();
            }

            protected override AuthenticationHandler<WebApi.AuthenticationOptions> CreateHandler(HttpClient httpClient)
            {
                var endpointBuilder = new EndpointBuilder();
                var ticketProvider = new TicketProvider(endpointBuilder, httpClient);
                var identityFactory = new WebApi.ClaimsIdentityFactory(Options.AuthenticationType);
                return new WebApi.AuthenticationHandler(_logger, ticketProvider, endpointBuilder, identityFactory);
            }
        }

        public class TicketProvider : WebApi.AuthenticationTicketProvider
        {
            public TicketProvider(IEndpointBuilder uriBuilder, HttpClient httpClient)
                : base(ProviderName, XmlSchemaString, uriBuilder, httpClient)
            {
            }

            protected override Task<IEnumerable<Claim>> GenerateClaimsAsync(string responseContent)
            {
                var response = JsonConvert.DeserializeObject<dynamic>(responseContent);
                string accessToken = response.access_token;
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var results = GetClaims(accessToken);
                return Task.FromResult(results);
            }

            private IEnumerable<Claim> GetClaims(string accessToken)
            {
                yield return new Claim(ServiceClaimTypes.ProviderAccessToken,
                    accessToken, Issuer, Provider);
            }
        }

        public class LoginProvider : WebApi.LoginProvider<AuthenticationMiddleware>
        {
            public LoginProvider(IServiceTokenHandler tokenHandler)
                : base(ProviderName, tokenHandler)
            {
            }

            protected override WebApi.AuthenticationOptions CreateAuthenticationOptions()
            {
                var options = new WebApi.AuthenticationOptions(ProviderName, XmlSchemaString,
                    TokenEndpoint, AuthEndpoint);
                options.Scope.Add("app");
                return options;
            }
        }
    }
}