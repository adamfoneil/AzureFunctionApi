using AO.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpData.Server
{
    /// <summary>
    /// used with http trigger Azure Functions to handle CRUD operations
    /// </summary>
    public abstract class AzureHttpFunction<TModel, TKey, TUser> where TModel : IModel<TKey> where TUser : IUserBase
    {
        protected readonly JsonApiRequest _request;
        protected readonly ILogger _logger;        
        protected readonly IRepository<TModel, TKey, TUser> _repository;

        public AzureHttpFunction(JsonApiRequest request, ILogger logger, IRepository<TModel, TKey, TUser> repository)
        {
            _request = request;
            _logger = logger;            
            _repository = repository;
        }

        protected abstract string HandlerName { get; }

        protected abstract Task<(bool success, TUser user)> AuthenticateAsync(JsonApiRequest request);

        protected abstract TKey GetId(JsonApiRequest request);

        public async Task<IActionResult> ExecuteAsync()
        {
            var auth = await AuthenticateAsync(_request);
            if (!auth.success) return new BadRequestObjectResult("Not authenticated.");            

            try
            {                
                TModel result;

                if (_request.Action == Action.Get)
                {
                    var id = GetId(_request);
                    result = await _repository.GetAsync(auth.user, id);
                    return new OkObjectResult(result);
                }

                if (_request.Action == Action.Save)
                {
                    var model = JsonSerializer.Deserialize<TModel>(_request.Body);
                    result = await _repository.SaveAsync(auth.user, model);
                    return new OkObjectResult(result);                    
                }

                if (_request.Action == Action.Delete)
                {
                    var key = GetId(_request);
                    await _repository.DeleteAsync(auth.user, key);
                    return new OkResult();
                }

                throw new Exception($"Unsupported API action {_request.Action}");
            }
            catch (Exception exc)
            {
                var message = $"Error executing {HandlerName} crud handler {_request.Action} method: {exc.Message}";
                _logger.LogError(message, exc);
                return new BadRequestObjectResult(message);
            }            
        }
    }
}
