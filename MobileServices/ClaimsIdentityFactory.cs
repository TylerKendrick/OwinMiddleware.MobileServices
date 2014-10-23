using System.Security.Claims;
using MobileServiceProviders;

namespace MobileServices
{
    public static partial class WebApi
    {
        public class ClaimsIdentityFactory : IClaimsIdentityFactory
        {
            private readonly string _signInAsAuthenticationType;

            public ClaimsIdentityFactory(string signInAsAuthenticationType)
            {
                _signInAsAuthenticationType = signInAsAuthenticationType;
            }

            public virtual ClaimsIdentity Create(ClaimsIdentity identity)
            {
                return new ClaimsIdentity(identity.Claims, _signInAsAuthenticationType, identity.NameClaimType,
                    identity.RoleClaimType);
            }
        }
    }
}