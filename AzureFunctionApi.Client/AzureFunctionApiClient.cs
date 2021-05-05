using AO.Models.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AzureFunctionApi.Client
{
    public class AzureFunctionApiClient<TKey>
    {
        private static HttpClient _client = new HttpClient();

        private readonly ILogger _logger;

        public AzureFunctionApiClient(string host, ILogger logger = null)
        {
            Host = host;
            _logger = logger;
        }

        public string Host { get; }
      
        public async Task<TModel> PostAsync<TModel>(TModel model) where TModel : IModel<TKey>
        {
            //_logger?.LogTrace()

            var response = await _client.PostAsJsonAsync(RouteUrl<TModel>(), model);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TModel>();
        }

        public async Task<TModel> PutAsync<TModel>(TModel model) where TModel : IModel<TKey>
        {
            var response = await _client.PutAsJsonAsync(RouteUrl<TModel>(), model);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TModel>();
        }

        public async Task<TModel> GetAsync<TModel>(TKey id) where TModel : IModel<TKey>
        {
            var url = RouteUrlWithId<TModel>(id);
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TModel>();
        }

        public async Task DeleteAsync<TModel>(TModel model) where TModel : IModel<TKey>
        {
            var url = RouteUrlWithId(model);
            var response = await _client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        private string RouteUrlWithId<TModel>(TModel model) where TModel : IModel<TKey> => RouteUrlWithId<TModel>(model.Id);

        private string RouteUrlWithId<TModel>(TKey id) where TModel : IModel<TKey> => $"{RouteUrl<TModel>()}?id={id}";

        private string RouteUrl<TModel>() where TModel : IModel<TKey> => Host + $"/{typeof(TModel).Name}";
    }
}
