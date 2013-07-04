using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlashCards.Model;

namespace FlashCards.Data
{
    public class RepositoryBase<T> : SqliteDbComponent, IRepository<T> where T : Entity, new()
    {
        public RepositoryBase()
        {
            Db = OpenConnection();
        }

        public T Find(int id)
        {
            return Db.Find<T>(id);
        }

        public IEnumerable<T> All
        {
            get
            {
                return Db.Table<T>();
            }
        }

        public int InsertOrUpdate(T item)
        {
            return item.Id == 0 ? Db.Insert(item) : Db.Update(item);
        }

        public void Delete(int id)
        {
            Db.Delete(Find(id));
        }

        public void Delete(T item)
        {
            Db.Delete(item);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}