using System.Collections.Generic;
using System.Linq;
using NPoco;

namespace NPocoCachedRepository
{
    public class NPocoCachedRepository<T>: ICachedRepository<T>
    {

        protected Cache<T> _cache;
        protected IDatabase _dataBase;
        protected readonly string _tableName;

        public NPocoCachedRepository(IDatabase db)
        {
            _dataBase = db;
            _cache = new Cache<T>();
            _tableName = _dataBase.PocoDataFactory.ForType(typeof(T)).TableInfo.TableName;
        }

        public virtual object Add(T instance)
        {
            _cache.Add(instance);
            return instance;
        }
        public virtual void Update(T instance)
        {
            _cache.Update(instance);
        }

        public virtual void Remove(T instance)
        {
            _cache.Remove(instance);
        }
        public virtual void RemoveById(object id)
        {
            T instance = GetById(id);
            if (instance != null)
                _cache.Remove(instance);
        }

        public virtual T GetById(object id)
        {
            return _dataBase.SingleById<T>(id);
        }
        public virtual IEnumerable<T> GetAll()
        {
            var objList = _dataBase.Fetch<T>("SELECT * FROM " + _tableName);
            return ApplyCachedChanges(objList);
        }
        public virtual IQueryable<T> QueryDb()
        {
            var objects = _dataBase.Query<T>("SELECT * FROM " + _tableName).AsQueryable<T>();
            return objects;
        }

        public virtual void Save()
        {
            using (var transaction = _dataBase.GetTransaction())
            {
                foreach (var addObj in _cache.ObjectsToAdd)
                    _dataBase.Insert(addObj);
                foreach (var updateObj in _cache.ObjectsToUpdate)
                    _dataBase.Update(updateObj);
                foreach (var deleteObj in _cache.ObjectsToRemove)
                    _dataBase.Delete<T>(deleteObj);
                transaction.Complete();
            }
            _cache.Clear();
        }
        public virtual void Rollback()
        {
            _cache.Clear();
        }

        private IList<T> ApplyCachedChanges(IList<T> objList)
        {

            foreach (var objToAdd in _cache.ObjectsToAdd)
            {
                objList.Add(objToAdd);
            }
            foreach (var objToUpdate in _cache.ObjectsToUpdate)
            {
                objList.Remove(objToUpdate);
                objList.Add(objToUpdate);
            }    
            foreach (var objToRemove in _cache.ObjectsToRemove)
            {
                if (objList.Contains(objToRemove))
                    objList.Remove(objToRemove);
            }

            return objList;
        }

    }
}
