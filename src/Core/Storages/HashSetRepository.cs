using System.Collections;
using System.Collections.Generic;
using MoreLinq;

namespace Core.Storages
{
    public class HashSetRepository<T> : Repository, ISet<T>, IReadOnlyCollection<T>
    {
        private readonly HashSet<T> hashSet = new();
        public int Count => ((ICollection<T>) this.hashSet).Count;
        public bool IsReadOnly => ((ICollection<T>) this.hashSet).IsReadOnly;

        public bool Add(T item)
        {
            return ((ISet<T>) this.hashSet).Add(item);
        }

        public void Clear()
        {
            ((ICollection<T>) this.hashSet).Clear();
        }

        public bool Contains(T item)
        {
            return ((ICollection<T>) this.hashSet).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>) this.hashSet).CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            ((ISet<T>) this.hashSet).ExceptWith(other);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) this.hashSet).GetEnumerator();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            ((ISet<T>) this.hashSet).IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return ((ISet<T>) this.hashSet).IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return ((ISet<T>) this.hashSet).IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return ((ISet<T>) this.hashSet).IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return ((ISet<T>) this.hashSet).IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return ((ISet<T>) this.hashSet).Overlaps(other);
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>) this.hashSet).Remove(item);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return ((ISet<T>) this.hashSet).SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            ((ISet<T>) this.hashSet).SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            ((ISet<T>) this.hashSet).UnionWith(other);
        }

        void ICollection<T>.Add(T item)
        {
            ((ICollection<T>) this.hashSet).Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this.hashSet).GetEnumerator();
        }

        public void AddRange(IEnumerable<T> items)
        {
            this.hashSet.UnionWith(items);
        }

        protected override object GetDataToSave()
        {
            return this.hashSet.OrderBy(x => x, OrderByDirection.Ascending);
        }

        protected override void LoadData(string fileName)
        {
            JsonHelper.Populate(fileName, this);
        }
    }
}