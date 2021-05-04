All of my Blazor work has been in the Server-hosted model, but I want to get into WebAssembly, because I think this is ultimately where I should be heading. The main reasons are to open my work to cheaper hosting models and to expand my overall reach in the Blazor space. By targeting WebAssembly, I could host apps on as a static site, then use Azure Functions as the backend. This is much more cost effective than Azure App Service I believe.

The goal for this repo, therefore, is to work out the Azure Function integration in terms of an [HttpCrudClient](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Client/HttpCrudClient.cs) and [CrudHandler](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Server/CrudHandler.cs). I've done a lot of work in the relational data access space (via Dapper), and this is the first time I've really considered decoupling from that to some degree. I still envision relational data access in the backend, but WASM apps won't make a direct database connection like Server-based apps do. So, this requires some different thinking, but I still put a merciless emphasis on convention, simplicity, and testability.


# HttpData.Client.HttpCrudClient [HttpCrudClient.cs](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Client/HttpCrudClient.cs#L9)
## Properties
- string [Host](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Client/HttpCrudClient.cs#L21)
## Methods
- Task\<TModel\> [PostAsync](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Client/HttpCrudClient.cs#L23)<TModel>
 (TModel model)
- Task\<TModel\> [PutAsync](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Client/HttpCrudClient.cs#L32)<TModel>
 (TModel model)
- Task\<TModel\> [GetAsync](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Client/HttpCrudClient.cs#L39)<TKey>
 (TKey id)
- Task [DeleteAsync](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Client/HttpCrudClient.cs#L47)<TModel>
 (TModel model)

# HttpData.Server.CrudHandler [CrudHandler.cs](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Server/CrudHandler.cs#L14)
## Methods
- Task\<IActionResult\> [ExecuteAsync](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Server/CrudHandler.cs#L33)
 ()

# HttpData.Server.Repository [Repository.cs](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Server/Repository.cs#L8)
## Methods
- Task\<TModel\> [CreateAsync](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Server/Repository.cs#L10)<TModel>
 (TModel model)
- Task [DeleteAsync](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Server/Repository.cs#L15)
 (int id)
- Task\<TModel\> [GetByIdAsync](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Server/Repository.cs#L20)
 (int id)
- Task\<TModel\> [UpdateAsync](https://github.com/adamfoneil/HttpData/blob/master/HttpData.Server/Repository.cs#L25)<TModel>
 (TModel model)
