using AO.Models.Interfaces;
using Dapper.CX.SqlServer.Extensions.Int;
using HttpData.Server;
using Microsoft.AspNetCore.Http;
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
        public SampleFunction(HttpRequest request, ILogger logger, Repository<TModel> repository) : base(request, logger, repository)
        {
        }

        protected override string HandlerName => nameof(SampleFunction<TModel>);

        protected override async Task<(bool success, SampleUser user)> AuthenticateAsync(HttpRequest request) =>
            await Task.FromResult((true, new SampleUser() { Name = "test" }));

        protected override int GetId(HttpRequest request) => int.Parse(request.Query["id"]);
    }

    public class Repository<TModel> : IRepository<TModel, int, SampleUser> where TModel : IModel<int>
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task DeleteAsync(SampleUser user, int id)
        {
            using var cn = new SqlConnection(_connectionString);
            await cn.DeleteAsync<TModel>(id, user: user);
        }

        public async Task<TModel> GetAsync(SampleUser user, int id)
        {
            using var cn = new SqlConnection(_connectionString);
            return await cn.GetAsync<TModel>(id, user: user);
        }

        public async Task<TModel> SaveAsync(SampleUser user, TModel model)
        {
            using var cn = new SqlConnection(_connectionString);
            var id = await cn.SaveAsync(model, user: user);
            return await cn.GetAsync<TModel>(id);
        }

        //private bool IsNew(TModel model) => model.Id.Equals(default);
    }
}
