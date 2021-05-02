using HttpData.Server.Interfaces;
using HttpData.Shared.Interfaces;
using System;
using System.Threading.Tasks;

namespace HttpData.Server
{
    public class Repository<TModel> : IRepository<TModel, int> where TModel : IModel<int>
    {
        public Task<TModel> CreateAsync(TModel model)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> UpdateAsync(TModel model)
        {
            throw new NotImplementedException();
        }
    }
}
