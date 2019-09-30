using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Service.AllRepository
{
    public interface IAllRepository <T> where T:class
    {
        IEnumerable<T> GetAll();
        T GetById(int modelId);
        
        void InsertModel(T model);
        void DeleteModel(int modelId);
        void UpdatModel(T model);
        void Save();
    }
}
