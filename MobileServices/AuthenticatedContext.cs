using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace MobileServices
{
    public static partial class WebApi
    {
        public class AuthenticatedContext : Microsoft.Owin.Security.Provider.BaseContext
        {
            public AuthenticatedContext(string name, IOwinContext context)
                : base(context)
            {
                Id = name;
            }

            public ClaimsIdentity Identity { get; set; }
            public string Id { get; private set; }
            public AuthenticationProperties Properties { get; set; }
        }
    }
}