using HttpData.Shared.Interfaces;
using System;
using System.Threading.Tasks;

namespace HttpData.Server
{
    public class HttpCrudServer<TKey>
    {
        public async Task<TModel> OnGetAsync<TModel>(TKey id) where TModel : IModel<TKey>
        {
            throw new NotImplementedException();
        }

        public async Task<TModel> OnPostAsync<TModel>(TModel model) where TModel : IModel<TKey>
        {
            if (IsNew(model))
            {
                OnInsert(model);

                throw new NotImplementedException();
            }
            else
            {
                return await OnPutAsync(model);
            }
        }

        public async Task<TModel> OnPutAsync<TModel>(TModel model) where TModel : IModel<TKey>
        {
            OnUpdate(model);

            throw new NotImplementedException();
        }

        protected virtual void OnUpdate<TModel>(TModel model) where TModel : IModel<TKey>
        {
            // do nothing by default
        }

        protected virtual void OnInsert<TModel>(TModel model) where TModel : IModel<TKey>
        {
            // do nothing by default
        }

        public async Task OnDeleteAsync<TModel>(TModel model) where TModel : IModel<TKey>
        {
            throw new NotImplementedException();
        }

        private bool IsNew<TModel>(TModel model) where TModel : IModel<TKey> => model.Id.Equals(default);
    }
}
