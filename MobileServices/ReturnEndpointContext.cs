using Microsoft.Owin;
using Microsoft.Owin.Security;
using MobileServiceProviders;

namespace MobileServices
{
    public static partial class WebApi
    {
        public class ReturnEndpointContext : Microsoft.Owin.Security.Provider.ReturnEndpointContext
        {
            public ReturnEndpointContext(IOwinContext context, AuthenticationTicket ticket,
                IClaimsIdentityFactory claimsIdentityFactory)
                : base(context, ticket)
            {
                if (SignInAsAuthenticationType != null && Identity != null)
                {
                    var identity = Identity.AuthenticationType == SignInAsAuthenticationType
                        ? claimsIdentityFactory.Create(Identity)
                        : Identity;
                    context.Authentication.SignIn(Properties, identity);
                }
            }
        }
    }
}