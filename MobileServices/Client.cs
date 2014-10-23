using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Owin;
using Microsoft.Owin;
using MobileServiceProviders;
using Newtonsoft.Json;

namespace MobileServices
{
    public static partial class WebApi
    {
        public class Client : IClient
        {
            private readonly Uri _baseUri;
            private readonly HttpClient _client;

            public Client(Uri baseUri, HttpMessageHandler handler = null)
            {
                _baseUri = baseUri;
                handler = handler ?? new PassiveAuthenticationMessageHandler();
                _client = new HttpClient(handler);
            }

            protected virtual Uri GenerateUri(PathString pathString,
                ICollection<KeyValuePair<string, string>> parameters = null)
            {
                return WebApiHelper.GenerateUri(_baseUri, pathString, parameters);
            }

            public async Task<T> GetAsync<T>(PathString pathString,
                ICollection<KeyValuePair<string, string>> parameters = null)
            {
                var requestUri = GenerateUri(pathString, parameters);
                return await _client.GetAsJsonAsync<T>(requestUri);
            }

            public async Task<dynamic> GetAsync(PathString pathString,
                ICollection<KeyValuePair<string, string>> parameters = null)
            {
                return await GetAsync<dynamic>(pathString, parameters);
            }

            public async Task<T> PostAsync<T>(PathString pathString, dynamic entity)
            {
                var requestUri = GenerateUri(pathString);
                string entityJson = JsonConvert.SerializeObject(entity);
                var response = await _client.PostAsJsonAsync(requestUri, entityJson);
                return await response.Content.ReadAsJsonAsync<T>();
            }

            public async Task<dynamic> PostAsync(PathString pathString, dynamic entity)
            {
                return await PostAsync<dynamic>(pathString, entity);
            }

            public async Task<T> PutAsync<T>(PathString pathString, dynamic entity)
            {
                var requestUri = GenerateUri(pathString);
                string entityJson = JsonConvert.SerializeObject(entity);
                var response = await _client.PutAsJsonAsync(requestUri, entityJson);
                return await response.Content.ReadAsJsonAsync<T>();
            }

            public async Task<dynamic> PutAsync(PathString pathString, dynamic entity)
            {
                return await PutAsync<dynamic>(pathString, entity);
            }

            public async Task<T> DeleteAsync<T>(PathString pathString)
            {
                var requestUri = GenerateUri(pathString);
                var response = await _client.DeleteAsync(requestUri);
                return await response.Content.ReadAsJsonAsync<T>();
            }

            public async Task<dynamic> DeleteAsync(PathString pathString)
            {
                return await DeleteAsync<dynamic>(pathString);
            }
        }
    }
}