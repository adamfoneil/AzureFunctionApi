using HttpData.Shared.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HttpData.Client
{
    public class HttpCrudClient<TKey>
    {
        private static HttpClient _client = new HttpClient();

        public HttpCrudClient(string host)
        {
            Host = host;
        }

        public string Host { get; }
      
        public async Task<TModel> PostAsync<TModel>(TModel model) where TModel : IModel<TKey>
        {
            var response = await _client.PostAsync(RouteUrl<TModel>(), JsonContent.Create(model));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TModel>();
        }

        public async Task<TModel> PutAsync<TModel>(TModel model) where TModel : IModel<TKey>
        {
            var response = await _client.PutAsync(RouteUrl<TModel>(), JsonContent.Create(model));
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

        private string RouteUrlWithId<TModel>(TKey id) where TModel : IModel<TKey> => $"{RouteUrl<TModel>()}/{id}";

        private string RouteUrl<TModel>() where TModel : IModel<TKey> => Host + $"/{typeof(TModel).Name}";
    }
}
