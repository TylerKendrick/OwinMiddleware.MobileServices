using System;

namespace MobileServices
{
    public interface IEndpointBuilder
    {
        Uri Authorization(string state, string scope, string redirectUri);
        Uri Token(string state, string code, string redirectUri);
    }
}