using System.Security.Claims;

namespace MobileServiceProviders
{
    public interface IClaimsIdentityFactory
    {
        ClaimsIdentity Create(ClaimsIdentity identity);
    }
}