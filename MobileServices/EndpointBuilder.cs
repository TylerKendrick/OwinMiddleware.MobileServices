using System;
using System.Collections.Generic;
using MobileServiceProviders;

namespace MobileServices
{
    public static partial class WebApi
    {
        public abstract class EndpointBuilder : IEndpointBuilder
        {
            private readonly Uri _authUri;
            private readonly Uri _tokenUri;

            protected EndpointBuilder(Uri authUri, Uri tokenUri)
            {
                _authUri = authUri;
                _tokenUri = tokenUri;
            }

            public Uri Authorization(string state, string scope, string redirectUri)
            {
                var parameters = AuthorizationParameters(state, scope, redirectUri);
                return WebApiHelper.GenerateUri(_authUri, parameters);
            }

            public Uri Token(string state, string code, string redirectUri)
            {
                var parameters = TokenParameters(state, code, redirectUri);
                return WebApiHelper.GenerateUri(_tokenUri, parameters);
            }

            protected abstract IEnumerable<KeyValuePair<string, string>> AuthorizationParameters(string state,
                string scope, string redirectUri);

            protected abstract IEnumerable<KeyValuePair<string, string>> TokenParameters(string state,
                string code, string redirectUri);
        }
    }
}