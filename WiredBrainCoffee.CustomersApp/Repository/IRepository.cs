using System.Collections.Generic;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public interface IRepository<TModel> where TModel : class
    {
        IEnumerable<TModel> Add(TModel model);
        IEnumerable<TModel> GetAll();
        IEnumerable<TModel> Update(TModel model);
        IEnumerable<TModel> Delete(TModel model);
    }
}
