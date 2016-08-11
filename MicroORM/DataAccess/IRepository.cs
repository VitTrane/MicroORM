using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IRepository<TEntity,TKey>
    {
        TEntity GetById(TKey key);
        void Save(TEntity entity);
        void Delete(TKey key);
    }
}
