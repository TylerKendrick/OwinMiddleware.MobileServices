using System;
using System.Collections.Generic;
using System.Net.Http;
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
    public class Github
    {
        private const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
        private const string ClientId = "Client Application Id Goes Here";
        private const string ClientSecret = "Client Secret Key Goes Here";
        private static readonly Uri ApiEndpoint = new Uri("https://github.com");
        private static readonly PathString AuthPath = new PathString("/login/oauth/authorize");
        private static readonly PathString TokenPath = new PathString("/login/oauth/access_token");
        private const string ProviderName = "GitHub";

        public class EndpointBuilder : WebApi.EndpointBuilder
        {
            public EndpointBuilder()
                : base(ApiEndpoint, AuthPath, TokenPath)
            {
            }

            protected override IEnumerable<KeyValuePair<string, string>> TokenParameters(string state,
                string code, string redirectUri)
            {
                return new Dictionary<string, string>
                {
                    {"code", code},
                    {"redirect_uri", redirectUri},
                    {"client_id", ClientId},
                    {"client_secret", ClientSecret}
                };
            }

            protected override IEnumerable<KeyValuePair<string, string>> AuthorizationParameters(string state, string scope,
                string redirectUri)
            {
                return new Dictionary<string, string>
                {
                    {"client_id", ClientId},
                    {"redirect_uri", redirectUri},
                    {"scope", string.Join(",", scope)},
                    {"state", state}
                };
            }
        }

        public class TicketProvider : WebApi.AuthenticationTicketProvider
        {
            public TicketProvider(IEndpointBuilder uriBuilder, HttpClient httpClient)
                : base(ProviderName, XmlSchemaString, uriBuilder, httpClient)
            {
            }

            protected async override Task<IEnumerable<Claim>> GenerateClaimsAsync(string responseContent)
            {
                var response = JsonConvert.DeserializeObject<dynamic>(responseContent);
                string accessToken = response.access_token;

                var requestUri = "https://api.github.com/user?access_token=" + Uri.EscapeDataString(accessToken);
                var json = await HttpClient.GetAsJsonAsync<dynamic>(requestUri);

                return new[]
                {
                    new Claim(ServiceClaimTypes.ProviderAccessToken, accessToken, XmlSchemaString, ProviderName),
                    new Claim(ClaimTypes.NameIdentifier, json.id, XmlSchemaString, ProviderName), 
                    new Claim(ClaimsIdentity.DefaultNameClaimType, json.username, XmlSchemaString, ProviderName),
                    new Claim("urn:github:name", json.name, XmlSchemaString, ProviderName), 
                    new Claim("urn:github:url", json.link, XmlSchemaString, ProviderName)
                };
            }
        }
        public class Middleware : WebApi.AuthenticationMiddleware
        {
            private readonly ILogger _logger;

            public Middleware(OwinMiddleware next, IAppBuilder appBuilder, WebApi.AuthenticationOptions options)
                : base(next, appBuilder, options, typeof(Middleware).FullName)
            {
                _logger = appBuilder.CreateLogger<Middleware>();
            }

            protected override AuthenticationHandler<WebApi.AuthenticationOptions> CreateHandler(HttpClient httpClient)
            {
                var endpointBuilder = new EndpointBuilder();
                var ticketProvider = new TicketProvider(endpointBuilder, httpClient);
                var identityFactory = new WebApi.ClaimsIdentityFactory(ProviderName);
                return new WebApi.AuthenticationHandler(_logger, ticketProvider, endpointBuilder, identityFactory);
            }
        }

        public class LoginProvider : WebApi.LoginProvider<Middleware>
        {
            public LoginProvider(IServiceTokenHandler tokenHandler)
                : base(ProviderName, tokenHandler)
            {
            }

            protected override WebApi.AuthenticationOptions CreateAuthenticationOptions()
            {
                return new WebApi.AuthenticationOptions(ProviderName, XmlSchemaString,
                    ApiEndpoint, TokenPath, AuthPath);
            }
        }
    }
}