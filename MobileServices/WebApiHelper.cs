using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace MobileServiceProviders
{
    public static class WebApiHelper
    {
        public static string BaseUrl(this IOwinRequest request)
        {
            return request.Scheme +
                   Uri.SchemeDelimiter +
                   request.Host;
        }

        public static string FromPath(this IOwinRequest request, PathString path)
        {
            return BaseUrl(request) + path;
        }

        public static string FromPath(this IOwinRequest request, string path)
        {
            return FromPath(request, new PathString(path));
        }

        public static string Parameterize(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var keyValuePairs = parameters as KeyValuePair<string, string>[] ?? parameters.ToArray();
            return parameters != null && keyValuePairs.Any()
                ? "?" + String.Join("&", keyValuePairs.Select(x =>
                    String.Format("{0}={1}", x.Key, Uri.EscapeDataString(x.Value))))
                : null;
        }

        public static Uri GenerateUri(Uri baseUri, PathString pathString,
            IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            parameters = parameters ?? new Dictionary<string, string>();
            var requestQuery = pathString.Value + Parameterize(parameters);
            return new Uri(baseUri, requestQuery);
        }
        public static Uri GenerateUri(Uri baseUri,
            IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            parameters = parameters ?? new Dictionary<string, string>();
            return new Uri(baseUri, Parameterize(parameters));
        }

        public static PathString CombinePaths(this PathString pathString, params string[] strings)
        {
            return strings
                .Select(x => new PathString("/" + x))
                .Aggregate(pathString, (x, y) => x.Add(y));
        }

        public static async Task<T> GetAsJsonAsync<T>(this HttpClient httpClient, string requestUri)
        {
            var response = await httpClient.GetStringAsync(requestUri);
            return JsonConvert.DeserializeObject<T>(response);
        }

        public static async Task<T> GetAsJsonAsync<T>(this HttpClient httpClient, Uri requestUri)
        {
            var response = await httpClient.GetStringAsync(requestUri);
            return JsonConvert.DeserializeObject<T>(response);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent httpContent)
        {
            var response = await httpContent.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(response);
        }

        public static string BuildRedirectUri(this IOwinRequest request, PathString callbackPath)
        {
            return request.Scheme + Uri.SchemeDelimiter +
                request.Host + request.PathBase + callbackPath;
        }
    }
}