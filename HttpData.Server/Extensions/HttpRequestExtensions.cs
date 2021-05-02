using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreNotify.Functions.Helpers
{
    public static class HttpRequestExtensions
    {
        public static async Task<(T item, string json)> DeserializeJsonAsync<T>(this HttpRequest request, ILogger log = null)
        {
            var json = await ReadBodyAsync(request);

            log?.LogDebug(json);

            return (JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }), json);
        }

        public static async Task<T> DeserializeAsync<T>(this HttpRequest request)
        {
            return (await DeserializeJsonAsync<T>(request)).item;
        }

        public static async Task<string> ReadBodyAsync(this HttpRequest request) =>
            await new StreamReader(request.Body).ReadToEndAsync();

        public static (string name, string key) GetCredentials(this HttpRequest request)
        {
            try
            {
                return (request.Headers["account"], request.Headers["key"]);
            }
            catch (Exception exc)
            {
                throw new Exception($"Error getting credentials: {exc.Message}");
            }
        }        
    }
}
