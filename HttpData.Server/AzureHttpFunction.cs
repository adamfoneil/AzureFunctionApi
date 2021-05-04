using AO.Models.Interfaces;
using CoreNotify.Functions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HttpData.Server
{
    /// <summary>
    /// used with http trigger Azure Functions to handle CRUD operations
    /// </summary>
    public abstract class AzureHttpFunction<TModel, TKey, TUser> where TModel : IModel<TKey> where TUser : IUserBase
    {
        protected readonly HttpRequest _request;
        protected readonly ILogger _logger;        
        protected readonly IRepository<TModel, TKey, TUser> _repository;

        public AzureHttpFunction(HttpRequest request, ILogger logger, IRepository<TModel, TKey, TUser> repository)
        {
            _request = request;
            _logger = logger;            
            _repository = repository;
        }

        protected abstract string HandlerName { get; }

        protected abstract Task<(bool success, TUser user)> AuthenticateAsync(HttpRequest request);

        protected abstract TKey GetId(HttpRequest request);

        public async Task<IActionResult> ExecuteAsync()
        {
            var auth = await AuthenticateAsync(_request);
            if (!auth.success) return new BadRequestObjectResult("Not authenticated.");            

            try
            {                
                TModel result;

                if (HttpMethods.IsGet(_request.Method))
                {
                    var id = GetId(_request);
                    result = await _repository.GetAsync(auth.user, id);
                    return new OkObjectResult(result);
                }

                if (HttpMethods.IsPost(_request.Method) || HttpMethods.IsPut(_request.Method) || HttpMethods.IsPatch(_request.Method))
                {
                    var model = await _request.DeserializeAsync<TModel>();
                    result = await _repository.SaveAsync(auth.user, model);
                    return new OkObjectResult(result);                    
                }

                if (HttpMethods.IsDelete(_request.Method))
                {
                    var key = GetId(_request);
                    await _repository.DeleteAsync(auth.user, key);
                    return new OkResult();
                }

                throw new Exception($"Unsupported method {_request.Method}");
            }
            catch (Exception exc)
            {
                var message = $"Error executing {HandlerName} crud handler {_request.Method} method: {exc.Message}";
                _logger.LogError(message, exc);
                return new BadRequestObjectResult(message);
            }            
        }
    }
}
