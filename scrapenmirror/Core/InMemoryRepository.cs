using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scrapenmirror.Core
{
    public class InMemoryRepository<TKey, TValue> : IRepository<TKey, TValue>
        where TValue : IRepositoryItem<TKey>
    {
        private readonly ConcurrentDictionary<TKey, TValue> items;

        public InMemoryRepository()
        {
            this.items = new ConcurrentDictionary<TKey, TValue>();
        }

        public TValue this[TKey id] =>
            (id != null && this.items.ContainsKey(id)) ? this.items[id] : default(TValue);

        public void Add(TValue item)
        {
            this.items.GetOrAdd(item.Id, item);
        }

        public bool Remove(TKey id)
        {
            TValue removed = default(TValue);
            return this.items.TryRemove(id, out removed);
        }

        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        public IEnumerable<TValue> Values => items.Values;

        public bool Remove(TValue item)
        {
            return this.Remove(item.Id);
        }

        public IEnumerable<TValue> Where(Func<TValue, bool> func)
        {
            return items.Values.Where(x => func(x));
        }

 
    }

    public interface IRepository<TKey, TValue>
    {
        void Add(TValue item);
        bool Remove(TKey item);
        bool Remove(TValue id);
        int Count { get; }
        TValue this[TKey id] { get; }
        IEnumerable<TValue> Where(Func<TValue, bool> func);
        IEnumerable<TValue> Values { get; }
    }

    public interface IRepositoryItem<T>
    {
        T Id { get; set; }

    }
}
