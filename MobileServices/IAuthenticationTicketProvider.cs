using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace MobileServices
{
    public interface IAuthenticationTicketProvider
    {
        Task<AuthenticationTicket> GenerateAuthenticationTicket(IOwinContext owinContext,
            IAuthenticationOptions options, AuthenticationProperties properties);
    }
}