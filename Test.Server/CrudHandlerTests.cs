using BlazorAO.Models;
using Dapper;
using HttpData.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServer.LocalDb;
using System.Text.Json;

namespace Test.Server
{
    [TestClass]
    public class CrudHandlerTests
    {
        const string DbName = "BlazorAO";

        [TestMethod]
        public void CreateWorkspace()
        {
            const string WsName = "whatever";

            using (var cn = LocalDb.GetConnection(DbName))
            {
                cn.Execute("DELETE [dbo].[Workspace] WHERE [Name]=@name", new { name = WsName });
            }

            var ws = new Workspace()
            {
                Name = WsName
            };
            var body = JsonSerializer.Serialize(ws);

            var request = JsonApiRequest.Create("adamo", Action.Save, body);
            var logger = LoggerFactory.Create(config => config.AddDebug()).CreateLogger("AzureHttpFunction");
            var repo = new Repository<Workspace>(LocalDb.GetConnectionString(DbName), logger);
            var function = new SampleFunction<Workspace>(request, logger, repo);

            var result = function.ExecuteAsync().Result as OkObjectResult;
            Assert.IsTrue((result.Value as Workspace).Name.Equals(WsName));
        }    
    }   
}
