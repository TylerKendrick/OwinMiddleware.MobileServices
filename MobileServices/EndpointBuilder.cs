using System;
using System.Collections.Generic;
using Microsoft.Owin;
using MobileServiceProviders;

namespace MobileServices
{
    public static partial class WebApi
    {
        public abstract class EndpointBuilder : IEndpointBuilder
        {
            private readonly Uri _baseUri;
            private readonly PathString _authorizationPath;
            private readonly PathString _tokenPath;

            protected EndpointBuilder(Uri baseUri, PathString authorizationPath, PathString tokenPath)
            {
                _baseUri = baseUri;
                _authorizationPath = authorizationPath;
                _tokenPath = tokenPath;
            }

            public Uri Authorization(string state, string scope, string redirectUri)
            {
                var parameters = AuthorizationParameters(state, scope, redirectUri);
                return WebApiHelper.GenerateUri(_baseUri, _authorizationPath, parameters);
            }

            public Uri Token(string state, string code, string redirectUri)
            {
                var parameters = TokenParameters(state, code, redirectUri);
                return WebApiHelper.GenerateUri(_baseUri, _tokenPath, parameters);
            }

            protected abstract IEnumerable<KeyValuePair<string, string>> AuthorizationParameters(string state,
                string scope, string redirectUri);

            protected abstract IEnumerable<KeyValuePair<string, string>> TokenParameters(string state,
                string code, string redirectUri);
        }
    }
}