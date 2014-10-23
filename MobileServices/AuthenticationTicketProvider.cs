using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MobileServiceProviders;

namespace MobileServices
{
    public static partial class WebApi
    {
        public class AuthenticationTicketProvider : IAuthenticationTicketProvider
        {
            private readonly IEndpointBuilder _uriBuilder;

            protected HttpClient HttpClient { get; private set; }
            protected string Provider { get; private set; }
            protected string Issuer { get; private set; }

            public AuthenticationTicketProvider(string providerName, string issuer,
                IEndpointBuilder uriBuilder, HttpClient httpClient)
            {
                _uriBuilder = uriBuilder;
                HttpClient = httpClient;
                Provider = providerName;
                Issuer = issuer;
            }

            private async Task<ClaimsIdentity> GenerateIdentity(string responseContent,
                IAuthenticationOptions options)
            {
                var identity = GenerateIdentity(options);
                var claims = await GenerateClaimsAsync(responseContent);
                identity.AddClaims(claims);
                return identity;
            }

            protected virtual ClaimsIdentity GenerateIdentity(IAuthenticationOptions options)
            {
                return new ClaimsIdentity(
                    options.AuthenticationType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            }

            protected virtual Task<IEnumerable<Claim>> GenerateClaimsAsync(string responseContent)
            {
                return Task.FromResult(System.Linq.Enumerable.Empty<Claim>());
            }

            private async Task<string> GetTokenResponse(string state, string code, string redirectUri)
            {
                var tokenRequestUri = _uriBuilder.Token(state, code, redirectUri);
                return await HttpClient.GetStringAsync(tokenRequestUri);
            }

            public async Task<AuthenticationTicket> GenerateAuthenticationTicket(IOwinContext owinContext,
                IAuthenticationOptions options, AuthenticationProperties properties)
            {
                var request = owinContext.Request;
                var code = request.Query["code"];
                var state = options.StateDataFormat.Protect(properties);

                var redirectUri = request.BuildRedirectUri(options.CallbackPath);
                var text = await GetTokenResponse(state, code, redirectUri);
                var identity = await GenerateIdentity(text, options);
                var context = CreateAuthenticatedContext(owinContext, options, properties, identity);

                return new AuthenticationTicket(context.Identity, context.Properties);
            }

            protected virtual AuthenticatedContext CreateAuthenticatedContext(IOwinContext owinContext,
                IAuthenticationOptions options, AuthenticationProperties properties, ClaimsIdentity identity)
            {
                return new AuthenticatedContext(options.SignInAsAuthenticationType, owinContext)
                {
                    Identity = identity,
                    Properties = properties
                };
            }
        }
    }
}