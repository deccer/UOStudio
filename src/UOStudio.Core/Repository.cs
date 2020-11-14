using System;
using System.Collections.Generic;
using System.Linq;

namespace UOStudio.Core
{
    public abstract class Repository<T> where T: Entity
    {
        protected readonly IList<T> _items;

        protected Repository() => _items = new List<T>();

        public IReadOnlyList<T> GetAll() => _items.ToList();

        public IReadOnlyList<TResult> GetAll<TResult>(Func<T, TResult> selector) =>
            _items
                .Select(selector)
                .ToList();

        public T GetByIndex(int index) => _items[index];

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Remove(T item)
        {
            _items.Remove(item);
        }

        public void Update(T item)
        {
            var i = _items.FirstOrDefault(ii => ii.Id == item.Id);
            i?.CopyFrom(item);
        }
    }
}
