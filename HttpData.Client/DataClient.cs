using HttpData.Shared.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpData.Client
{
    public class DataClient<TKey>
    {
        private static HttpClient _client = new HttpClient();

        public DataClient(string host)
        {
            Host = host;
        }

        public string Host { get; }
      
        public async Task<TModel> PostAsync<TModel>(TModel model) where TModel : IModel<TKey>
        {
            var response = await _client.PostAsync(Host, JsonContent.Create(model));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TModel>();
        }

        public async Task<TModel> GetAsync<TModel>(TKey id)
        {
            var url = GetActionUrl<TModel>(id);
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TModel>();
        }

        public async Task DeleteAsync<TModel>(TModel model) where TModel : IModel<TKey>
        {
            var url = GetActionUrl(model);
            var response = await _client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        private string GetActionUrl<TModel>(TModel model) where TModel : IModel<TKey> => GetActionUrl<TModel>(model.Id);

        private string GetActionUrl<TModel>(TKey id) => Host + $"?type={typeof(TModel).Name}&id={id}";
    }
}
