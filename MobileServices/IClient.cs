using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace MobileServiceProviders
{
    public interface IClient
    {
        Task<T> GetAsync<T>(PathString pathString,
            ICollection<KeyValuePair<string, string>> parameters = null);

        Task<object> GetAsync(PathString pathString,
            ICollection<KeyValuePair<string, string>> parameters = null);

        Task<T> PostAsync<T>(PathString pathString, dynamic entity);
        Task<object> PostAsync(PathString pathString, dynamic entity);
        Task<T> PutAsync<T>(PathString pathString, dynamic entity);
        Task<object> PutAsync(PathString pathString, dynamic entity);
        Task<T> DeleteAsync<T>(PathString pathString);
        Task<object> DeleteAsync(PathString pathString);
    }
}