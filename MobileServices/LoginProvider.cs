using System.Security.Claims;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Newtonsoft.Json.Linq;
using Owin;

namespace MobileServices
{
    public static partial class WebApi
    {
        public abstract class LoginProvider<TMiddleware> : LoginProvider
        {
            private readonly string _name;

            protected LoginProvider(string name, IServiceTokenHandler tokenHandler)
                : base(tokenHandler)
            {
                _name = name;
            }

            public override sealed Microsoft.WindowsAzure.Mobile.Service.Security.ProviderCredentials CreateCredentials(ClaimsIdentity claimsIdentity)
            {
                return new ProviderCredentials(Name, TokenHandler, claimsIdentity);
            }

            public override sealed Microsoft.WindowsAzure.Mobile.Service.Security.ProviderCredentials ParseCredentials(JObject serialized)
            {
                return serialized.ToObject<ProviderCredentials>();
            }

            public override sealed void ConfigureMiddleware(IAppBuilder appBuilder, ServiceSettingsDictionary settings)
            {
                var options = CreateAuthenticationOptions();
                appBuilder.Use<TMiddleware>(appBuilder, options);
            }

            protected abstract AuthenticationOptions CreateAuthenticationOptions();

            public override sealed string Name
            {
                get { return _name; }
            }
        }
    }
}