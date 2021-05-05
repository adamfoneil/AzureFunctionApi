using AzureFunction.Server;
using BlazorAO.Models;
using Dapper;
using Dapper.CX.Extensions;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServer.LocalDb;
using System.Collections.Generic;
using System.Text.Json;

namespace Test.Server
{
    [TestClass]
    public class CrudHandlerTests
    {
        const string DbName = "BlazorAO";
        const string WsName = "whatever";

        [TestMethod]
        public void CreateWorkspace()
        {            
            var result = CreateWorkspaceInner(WsName);
            Assert.IsTrue(result.Name.Equals(WsName));
        }

        [TestMethod]
        public void UpdateWorkspace()
        {
            var result = CreateWorkspaceInner(WsName);
            result.Name = "New Name";
            var id = result.Id;

            DeleteWorkspace("New Name");
            var updated = PostWorkspaceInner(result);
            Assert.IsTrue(id == updated.Id);
            Assert.IsTrue(updated.Name.Equals("New Name"));
            Assert.IsTrue(updated.DateModified.HasValue);
        }

        [TestMethod]
        public void DeleteWorkspace()
        {
            var result = CreateWorkspaceInner(WsName);
            var id = result.Id;

            var request = JsonApiRequest.Create("adamo", RequestAction.Delete, null, query: new Dictionary<string, object>() { ["id"] = id });
            var logger = LoggerFactory.Create(config => config.AddDebug()).CreateLogger("AzureHttpFunction");
            var repo = new Repository<Workspace>(LocalDb.GetConnectionString(DbName), logger);
            var function = new SampleFunction<Workspace>(request, logger, repo);

            function.ExecuteAsync().Wait();

            using var cn = LocalDb.GetConnection(DbName);
            Assert.IsTrue(!cn.RowExistsAsync("[dbo].[Workspace] WHERE [Name]=@name", new { name = WsName }).Result);
        }

        private static Workspace CreateWorkspaceInner(string name)
        {
            DeleteWorkspace(name);

            var ws = new Workspace()
            {
                Name = name
            };

            return PostWorkspaceInner(ws);
        }

        private static void DeleteWorkspace(string name)
        {
            using (var cn = LocalDb.GetConnection(DbName))
            {
                cn.Execute("DELETE [dbo].[Workspace] WHERE [Name]=@name", new { name });
            }
        }

        private static Workspace PostWorkspaceInner(Workspace input)
        {
            var body = JsonSerializer.Serialize(input);
            var request = JsonApiRequest.Create("adamo", RequestAction.Save, body);
            var logger = LoggerFactory.Create(config => config.AddDebug()).CreateLogger("AzureHttpFunction");
            var repo = new Repository<Workspace>(LocalDb.GetConnectionString(DbName), logger);
            var function = new SampleFunction<Workspace>(request, logger, repo);

            return (function.ExecuteAsync().Result as OkObjectResult).Value as Workspace;
        }
    }   
}
