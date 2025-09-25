using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public interface IRepository<TModel> where TModel : class
    {
        IEnumerable<TModel> GetAll();
        void SaveAll(IEnumerable<TModel> model);
        void Add(TModel model);
        void Update(TModel model);
        void Delete(TModel model);
    }
}
