using System;
using System.Threading.Tasks;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using MobileServiceProviders;

namespace MobileServices
{
    public static partial class WebApi
    {
        public class AuthenticationHandler : AuthenticationHandler<AuthenticationOptions>
        {
            private readonly ILogger _logger;
            private readonly IAuthenticationTicketProvider _authenticationTicketProvider;
            private readonly IEndpointBuilder _uriBuilder;
            private readonly IClaimsIdentityFactory _claimsIdentityFactory;

            public AuthenticationHandler(ILogger logger,
                IAuthenticationTicketProvider authenticationTicketProvider,
                IEndpointBuilder uriBuilder, IClaimsIdentityFactory claimsIdentityFactory)
            {
                _logger = logger;
                _authenticationTicketProvider = authenticationTicketProvider;
                _uriBuilder = uriBuilder;
                _claimsIdentityFactory = claimsIdentityFactory;
            }

            protected override sealed async Task<AuthenticationTicket> AuthenticateCoreAsync()
            {
                AuthenticationProperties properties = null;

                try
                {
                    var query = Request.Query;
                    var state = query["state"];

                    properties = Options.StateDataFormat.Unprotect(state);

                    return await GenerateAuthenticationTicket(properties);
                }
                catch (Exception exception)
                {
                    _logger.WriteError(exception.Message);
                }

                return new AuthenticationTicket(null, properties);
            }

            protected override sealed Task ApplyResponseChallengeAsync()
            {
                if (Response.StatusCode == 401)
                {
                    var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

                    if (challenge != null)
                    {
                        ChallengeRedirect(challenge);
                    }
                }
                return Task.FromResult<object>(null);
            }

            public override sealed async Task<bool> InvokeAsync()
            {
                if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
                {
                    var ticket = await AuthenticateAsync();
                    var errors = await HandleErrors(ticket);
                    if (errors)
                    {
                        return true;
                    }

                    var context = CreateReturnEndpointContext(ticket);

                    return ReturnRedirect(context);
                }

                return false;
            }

            private async Task<AuthenticationTicket> GenerateAuthenticationTicket(AuthenticationProperties properties)
            {
                if (properties == null)
                {
                    return null;
                }

                // OAuth2 10.12 CSRF
                if (!ValidateCorrelationId(properties, _logger))
                {
                    return new AuthenticationTicket(null, properties);
                }

                return await _authenticationTicketProvider.GenerateAuthenticationTicket(Context, Options, properties);
            }

            private void ChallengeRedirect(AuthenticationResponseChallenge challenge)
            {
                var properties = challenge.Properties;
                if (string.IsNullOrEmpty(properties.RedirectUri))
                {
                    properties.RedirectUri = Request.Uri.AbsoluteUri;
                }

                // OAuth2 10.12 CSRF
                GenerateCorrelationId(properties);

                var scope = string.Join(",", Options.Scope);
                var state = Options.StateDataFormat.Protect(properties);
                var redirectUri = Request.BuildRedirectUri(Options.CallbackPath);
                var authorizationEndpoint = _uriBuilder.Authorization(state, scope, redirectUri);
                Response.Redirect(authorizationEndpoint.AbsoluteUri);
            }

            protected virtual Task<bool> HandleErrors(AuthenticationTicket authenticationTicket)
            {
                if (authenticationTicket == null)
                {
                    _logger.WriteWarning("Invalid return state, unable to redirect.");
                    Response.StatusCode = 500;
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }

            private bool ReturnRedirect(Microsoft.Owin.Security.Provider.ReturnEndpointContext context)
            {
                if (!context.IsRequestCompleted && context.RedirectUri != null)
                {
                    var redirectUri = context.RedirectUri;
                    if (context.Identity == null)
                    {
                        redirectUri = WebUtilities.AddQueryString(redirectUri, "error", "access_denied");
                    }
                    Response.Redirect(redirectUri);
                    context.RequestCompleted();
                }

                return context.IsRequestCompleted;
            }

            protected virtual ReturnEndpointContext CreateReturnEndpointContext(AuthenticationTicket ticket)
            {
                return new ReturnEndpointContext(Context, ticket, _claimsIdentityFactory)
                {
                    SignInAsAuthenticationType = Options.SignInAsAuthenticationType,
                    RedirectUri = ticket.Properties.RedirectUri
                };
            }
        }
    }
}