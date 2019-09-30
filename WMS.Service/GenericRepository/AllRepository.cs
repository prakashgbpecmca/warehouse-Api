using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Service.AllRepository;
using WMS.Service.DataAccess;

namespace WMS.Service.GenericRepository
{
    public class AllRepository<T> : IAllRepository<T> where T : class
    {
        private WMSEntities _context;
        private IDbSet<T> dbEntity;

        public AllRepository()
        {
            _context = new WMSEntities();
            dbEntity = _context.Set<T>();
        }

        public void DeleteModel(int modelId)
        {
            T model = dbEntity.Find(modelId);
            dbEntity.Remove(model);
        }

        public IEnumerable<T> GetAll()
        {
            return dbEntity.ToList();
        }

        public T GetById(int modelId)
        {
            return dbEntity.Find(modelId);
        }      

        public void InsertModel(T model)
        {
            dbEntity.Add(model);
        }

        public void UpdatModel(T model)
        {
            _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
