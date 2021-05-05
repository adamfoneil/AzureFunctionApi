using AO.Models.Interfaces;
using Dapper.CX.SqlServer.Extensions.Int;
using HttpData.Server;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Test.Server
{
    public class SampleUser : IUserBase
    {
        public string Name { get; set; }
        public DateTime LocalTime => DateTime.Now;
    }

    public class SampleFunction<TModel> : AzureHttpFunction<TModel, int, SampleUser> where TModel : IModel<int>
    {
        public SampleFunction(JsonApiRequest request, ILogger logger, Repository<TModel> repository) : base(request, logger, repository)
        {
        }

        protected override string HandlerName => nameof(SampleFunction<TModel>);

        protected override async Task<(bool success, SampleUser user)> AuthenticateAsync(JsonApiRequest request) =>
            await Task.FromResult((true, new SampleUser() { Name = request.UserName }));

        protected override int GetId(JsonApiRequest request) => int.Parse(request.Query["id"]);
    }

    public class Repository<TModel> : IRepository<TModel, int, SampleUser> where TModel : IModel<int>
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public Repository(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task DeleteAsync(SampleUser user, int id)
        {
            _logger.LogTrace($"{user.Name} deleting {typeof(TModel).Name} id = {id}");
            using var cn = new SqlConnection(_connectionString);
            await cn.DeleteAsync<TModel>(id, user: user);
        }

        public async Task<TModel> GetAsync(SampleUser user, int id)
        {
            _logger.LogTrace($"{user.Name} getting {typeof(TModel).Name} id = {id}");
            using var cn = new SqlConnection(_connectionString);
            return await cn.GetAsync<TModel>(id, user: user);
        }

        public async Task<TModel> SaveAsync(SampleUser user, TModel model)
        {
            _logger.LogTrace($"{user.Name} saving {typeof(TModel).Name} {model}");
            using var cn = new SqlConnection(_connectionString);

            int id;
            if (IsNew(model))
            {
                id = await cn.SaveAsync(model, user: user);
            }
            else
            {
                id = await cn.MergeAsync(model, user: user);
            }
            
            return await cn.GetAsync<TModel>(id);
        }

        private bool IsNew(TModel model) => model.Id.Equals(default);
    }
}
