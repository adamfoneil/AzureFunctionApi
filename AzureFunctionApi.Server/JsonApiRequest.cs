using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Server
{
    public enum RequestAction
    {
        Get,
        Save, // post, put, patch
        Delete
    }

    /// <summary>
    /// HttpRequest facade for ease of testing (without http client or server)    
    /// Assumes json payload always, which is why we don't inspect Form collection
    /// </summary>
    public class JsonApiRequest
    {
        public JsonApiRequest(string userName, RequestAction action, string body, Dictionary<string, StringValues> headers, Dictionary<string, StringValues> query)
        {
            Action = action;
            UserName = userName;
            Body = body;
            Headers = headers;
            Query = query;
        }      

        /// <summary>
        /// simpler initialization of Headers and Query
        /// </summary>
        public static JsonApiRequest Create(string userName, RequestAction action, string body, Dictionary<string, object> headers = null, Dictionary<string, object> query = null)
        {
            return new JsonApiRequest(userName, action, body, ConvertDictionary(headers), ConvertDictionary(query));

            Dictionary<string, StringValues> ConvertDictionary(Dictionary<string, object> dictionary) => 
                (dictionary != null) ?
                    dictionary.ToDictionary(item => item.Key, item => new StringValues(new string[] { item.Value.ToString() })) :
                    null;            
        }

        public static async Task<JsonApiRequest> FromHttpRequestAsync(HttpRequest request)
        {
            var body = await new StreamReader(request.Body).ReadToEndAsync();            

            return new JsonApiRequest(GetUserName(), GetAction(), body, ParseDictionary(request.Headers), ParseDictionary(request.Query));

            RequestAction GetAction() =>
                (HttpMethods.IsGet(request.Method)) ? RequestAction.Get :
                (HttpMethods.IsPost(request.Method) || HttpMethods.IsPut(request.Method) || HttpMethods.IsPatch(request.Method)) ? RequestAction.Save :
                (HttpMethods.IsDelete(request.Method)) ? RequestAction.Delete :
                throw new Exception($"Unsupported method: {request.Method}");

            string GetUserName()
            {
                try
                {
                    return request.HttpContext.User.Identity.Name;
                }
                catch
                {
                    return null;
                }
            }

            Dictionary<string, StringValues> ParseDictionary(IEnumerable<KeyValuePair<string, StringValues>> collection) => 
                collection
                    .GroupBy(item => item.Key)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => new StringValues(grp.SelectMany(item => item.Value).ToArray()));
        }

        public Dictionary<string, StringValues> Headers { get; }
        public Dictionary<string, StringValues> Query { get; }
        public RequestAction Action { get; }
        public string UserName { get; }
        public string Body { get; }
    }
}
