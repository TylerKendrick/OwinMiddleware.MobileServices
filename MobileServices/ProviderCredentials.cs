using System.Security.Claims;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace MobileServices
{
    public static partial class WebApi
    {
        public class ProviderCredentials : Microsoft.WindowsAzure.Mobile.Service.Security.ProviderCredentials
        {
            public string AccessToken { get; private set; }

            public ProviderCredentials(string providerName,
                IServiceTokenHandler tokenHandler, ClaimsIdentity claimsIdentity)
                : base(providerName)
            {
                var username = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                UserId = tokenHandler.CreateUserId(providerName, username != null ? username.Value : null);

                var accessToken = claimsIdentity.FindFirst(ServiceClaimTypes.ProviderAccessToken);
                AccessToken = accessToken != null ? accessToken.Value : null;
            }
        }
    }
}