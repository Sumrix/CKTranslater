using System.Collections;
using System.Collections.Generic;

namespace Translation.Storages
{
    public abstract class DictionaryRepository<TItem, TKey, TValue> : Repository, IEnumerable<TItem>
    {
        protected Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        ///     Gets or sets the value associated with the specified key
        /// </summary>
        /// <param name="key">The key of the value to get or set</param>
        /// <returns>The value associated with the specified key</returns>
        public TValue this[TKey key]
        {
            get => this.dictionary[key];
            set => this.dictionary[key] = value;
        }

        /// <summary>
        ///     Gets the number of items contained in the <see cref="DictionaryRepository{TItem, TKey, TValue}" />
        /// </summary>
        public int Count => this.dictionary.Count;

        /// <summary>
        ///     Return an enumerator that iterates through the <see cref="DictionaryRepository{TItem, TKey, TValue}" />.
        /// </summary>
        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (var keyValuePair in this.dictionary)
            {
                yield return this.KeyValuePair2Item(keyValuePair);
            }
        }

        /// <summary>
        ///     Return an enumerator that iterates through the <see cref="DictionaryRepository{TItem, TKey, TValue}" />.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected override object GetDataToSave()
        {
            return new SortedDictionary<TKey, TValue>(this.dictionary);
        }

        protected abstract TItem KeyValuePair2Item(KeyValuePair<TKey, TValue> keyValuePair);

        protected abstract KeyValuePair<TKey, TValue> Item2KeyValuePair(TItem item);

        /// <summary>
        ///     Add the specified <paramref name="key" /> and <paramref name="value" /> to the
        ///     <see cref="DictionaryRepository{TItem, TKey, TValue}" />.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(TKey key, TValue value)
        {
            this.dictionary.Add(key, value);
        }

        public void Add(TItem item)
        {
            var keyValuePair = this.Item2KeyValuePair(item);
            this.dictionary[keyValuePair.Key] = keyValuePair.Value;
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        ///     Removes all keys and values from the <see cref="DictionaryRepository{TItem, TKey, TValue}" />.
        /// </summary>
        public void Clear()
        {
            this.dictionary.Clear();
        }

        /// <summary>
        ///     Determines whether the <see cref="DictionaryRepository{TItem, TKey, TValue}" /> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="DictionaryRepository{TItem, TKey, TValue}" />.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="DictionaryRepository{TItem, TKey, TValue}" /> contains
        ///     an element with the specified <paramref name="key" />; otherwise <c>false</c>.
        /// </returns>
        public bool Contains(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }

        protected override void LoadData(string fileName)
        {
            this.dictionary = JsonHelper.Deserialize<Dictionary<TKey, TValue>>(fileName);
        }

        /// <summary>
        ///     Removes the value with the specified <paramref name="key" /> from the
        ///     <see cref="DictionaryRepository{TItem, TKey, TValue}" />
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element is successfully found and removed; otherwise <c>false</c>.</returns>
        public bool Remove(TKey key)
        {
            return this.dictionary.Remove(key);
        }

        /// <summary>
        ///     Gets the value specified with the <paramref name="key" />.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key, if the key is found;
        ///     otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <see cref="DictionaryRepository{TItem, TKey, TValue}" /> contains
        ///     an element with the specified <paramref name="key" />; otherwise <c>false</c>.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.dictionary.TryGetValue(key, out value);
        }
    }
}