using AO.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HttpData.Server
{
    public enum Action
    {
        Get,
        Save, // post, put, patch
        Delete
    }

    /// <summary>
    /// represents an http request boilded down to its representation as a request to update data.
    /// This is so API operations can be tested without an http client
    /// </summary>
    public class DataRequest
    {
        public DataRequest(string userName, Action action, string body, Dictionary<string, StringValues> headers, Dictionary<string, StringValues> query)
        {
            Action = action;
            UserName = userName;
            Body = body;
        }      

        public static async Task<DataRequest> FromHttpRequestAsync(HttpRequest request)
        {
            var body = await new StreamReader(request.Body).ReadToEndAsync();            

            return new DataRequest(GetUserName(), GetAction(), body, ParseDictionary(request.Headers), ParseDictionary(request.Query));

            Action GetAction() =>
                (HttpMethods.IsGet(request.Method)) ? Action.Get :
                (HttpMethods.IsPost(request.Method) || HttpMethods.IsPut(request.Method) || HttpMethods.IsPatch(request.Method)) ? Action.Save :
                (HttpMethods.IsDelete(request.Method)) ? Action.Delete :
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

            Dictionary<string, StringValues> ParseDictionary(IEnumerable<KeyValuePair<string, StringValues>> collection)
            {
                return collection
                    .GroupBy(item => item.Key)
                    .ToDictionary(
                        grp => grp.Key, 
                        grp => new StringValues(grp.SelectMany(item => item.Value).ToArray()));
            }
        }

        public Dictionary<string, StringValues> Headers { get; }
        public Dictionary<string, StringValues> Query { get; }
        public Action Action { get; }
        public string UserName { get; }
        public string Body { get; }
    }
}
