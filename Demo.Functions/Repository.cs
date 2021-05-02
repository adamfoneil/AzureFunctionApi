using Demo.Database;
using Demo.Functions.Helpers;
using HttpData.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Demo.Functions
{
    public static class Repository
    {
        [FunctionName("Repository")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = null)] HttpRequest request,
            ILogger log, ExecutionContext context) => await new FunctionHandler<int>(request, log, context.GetConfig()).DispatchAsync(                
                new Repository<Employee>(),
                new Repository<WorkHours>(),
                new Repository<Client>());        
    }
}

