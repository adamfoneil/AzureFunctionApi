using AO.Models.Interfaces;
using HttpData.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Test.Server
{
    [TestClass]
    public class CrudHandlerTests
    {
        private static HttpClient _client = new HttpClient();

        [TestMethod]
        public void TestMethod1()
        {
        }
    }

    public class SampleUser : IUserBase
    {
        public string Name { get; set; }
        public DateTime LocalTime => DateTime.Now;
    }

    public class SampleHandler<TModel> : CrudHandler<TModel, int, SampleUser> where TModel : IModel<int>
    {
        private readonly IConfiguration _config;

        public SampleHandler(HttpRequest request, ILogger logger, IConfiguration config, IRepository<TModel, int, SampleUser> repository) : base(request, logger, repository)
        {
            _config = config;
        }

        protected override string HandlerName => nameof(SampleHandler<TModel>);

        protected override async Task<(bool success, SampleUser user)> AuthenticateAsync(HttpRequest request) => 
            await Task.FromResult((true, new SampleUser() { Name = "test" }));

        protected override int GetId(HttpRequest request) => int.Parse(request.Query["id"]);        
    }
}
