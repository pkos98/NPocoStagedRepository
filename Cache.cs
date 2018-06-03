using System.Collections.Generic;

namespace NPocoCachedRepository
{
    public class Cache<T>
    {
        public IList<T> ObjectsToRemove { get; } = new List<T>();
        public IList<T> ObjectsToAdd { get; } = new List<T>();
        public IList<T> ObjectsToUpdate { get; } = new List<T>();

        public void Add(T instance)
        {
            if (ObjectsToRemove.Contains(instance))
                ObjectsToRemove.Remove(instance);
            if (!ObjectsToAdd.Contains(instance))
                ObjectsToAdd.Add(instance);
        }
        public void Remove(T instance)
        {
            if (ObjectsToAdd.Contains(instance))
                ObjectsToAdd.Remove(instance);
            if (ObjectsToUpdate.Contains(instance))
                ObjectsToUpdate.Remove(instance);
            if (!ObjectsToRemove.Contains(instance))
                ObjectsToRemove.Add(instance);
        }
        public void Update(T instance)
        {
            if (!ObjectsToUpdate.Contains(instance))
                ObjectsToUpdate.Add(instance);
        }

        public void Clear() {
            ObjectsToAdd.Clear();
            ObjectsToRemove.Clear();
            ObjectsToUpdate.Clear();
        }

    }
}
