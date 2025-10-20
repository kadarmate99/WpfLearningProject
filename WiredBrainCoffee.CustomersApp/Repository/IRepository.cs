using System.Collections.Generic;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public interface IRepository<TModel> where TModel : class
    {
        IEnumerable<TModel> GetAll();
        IEnumerable<TModel> SaveAll(IEnumerable<TModel> model);
        IEnumerable<TModel> Add(TModel model);
        IEnumerable<TModel> Update(TModel model);
        IEnumerable<TModel> Delete(TModel model);
    }
}
