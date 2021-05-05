using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace Test.Server
{
    [TestClass]
    public class CrudHandlerTests
    {
        private HttpServer _server;
        private HttpClient _client;

        public CrudHandlerTests()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("Default", "api/{controller}/{action}/{id}", defaults: new { id = RouteParameter.Optional });
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            _server = new HttpServer(config);
            _client = new HttpClient(_server);
        }

        [TestMethod]
        public void SampleCrudActions()
        {

        }

        public void Dispose()
        {
            if (_server != null) _server.Dispose();
        }
    }   
}
