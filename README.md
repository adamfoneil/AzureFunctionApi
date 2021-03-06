All of my Blazor work has been in the Server-hosted model, but I want to get into WebAssembly, because I think this is ultimately where I should be heading. The main reasons are to open my work to cheaper hosting models and to expand my overall reach in the Blazor space. By targeting WebAssembly, I could host apps on as a static site, then use Azure Functions as the backend. This is much more cost effective than Azure App Service I believe.

The goal for this repo, therefore, is to work out the Azure Function integration in terms of an [AzureFunctionApiClient](https://github.com/adamfoneil/AzureFunctionApi/blob/master/AzureFunctionApi.Client/AzureFunctionApiClient.cs) and [AzureFunctionHandler](https://github.com/adamfoneil/AzureFunctionApi/blob/master/AzureFunctionApi.Server/AzureFunctionHandler.cs). I've done a lot of work in the relational data access space (via Dapper), and this is the first time I've really considered decoupling from that to some degree. I still envision relational data access in the backend, but WASM apps won't make a direct database connection like Server-based apps do. So, this requires some different thinking, but I still put a merciless emphasis on convention, simplicity, and testability.

Note, I added a couple new interfaces to [AO.Models interfaces](https://github.com/adamfoneil/Models/tree/master/Models/Interfaces) that will be a part of this:
- [IModel](https://github.com/adamfoneil/Models/blob/master/Models/Interfaces/IModel.cs), which is a little funny because in the past I've shunned base class dependencies like this. I envision this as the root interface that any model class must implement. This is really just a way to ensure that entities/models have an `Id` property.
- [IRepository](https://github.com/adamfoneil/Models/blob/master/Models/Interfaces/IRepository.cs), which is a little funny because I've expressed concerns about code verbosity around the Repository Pattern. This is motivated by a concern to separate the data access from the HTTP interactions.

# AzureFunctionApi.Client.AzureFunctionApiClient [AzureFunctionApiClient.cs](https://github.com/adamfoneil/AzureFunctionApi/blob/master/AzureFunctionApi.Client/AzureFunctionApiClient.cs#L9)
## Properties
- string [Host](https://github.com/adamfoneil/AzureFunctionApi/blob/master/AzureFunctionApi.Client/AzureFunctionApiClient.cs#L26)
## Methods
- Task\<TModel\> [PostAsync](https://github.com/adamfoneil/AzureFunctionApi/blob/master/AzureFunctionApi.Client/AzureFunctionApiClient.cs#L28)<TModel>
 (TModel model)
- Task\<TModel\> [PutAsync](https://github.com/adamfoneil/AzureFunctionApi/blob/master/AzureFunctionApi.Client/AzureFunctionApiClient.cs#L37)<TModel>
 (TModel model)
- Task\<TModel\> [GetAsync](https://github.com/adamfoneil/AzureFunctionApi/blob/master/AzureFunctionApi.Client/AzureFunctionApiClient.cs#L44)<TKey>
 (TKey id)
- Task [DeleteAsync](https://github.com/adamfoneil/AzureFunctionApi/blob/master/AzureFunctionApi.Client/AzureFunctionApiClient.cs#L52)<TModel>
 (TModel model)

# AzureFunctionApi.Server.AzureFunctionHandler [AzureFunctionHandler.cs](https://github.com/adamfoneil/AzureFunctionApi/blob/master/AzureFunctionApi.Server/AzureFunctionHandler.cs#L14)
## Methods
- Task\<IActionResult\> [ExecuteAsync](https://github.com/adamfoneil/AzureFunctionApi/blob/master/AzureFunctionApi.Server/AzureFunctionHandler.cs#L33)
 ()
