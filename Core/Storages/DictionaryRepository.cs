using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Storages
{
    /// <summary>
    ///     Represents a cache-able collections of keys and values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public abstract class DictionaryRepository<TKey, TValue> : Repository
    {
        /// <summary>
        ///     Inner data collection
        /// </summary>
        protected Dictionary<TKey, TValue> dictionary = new();

        /// <summary>
        ///     Gets the number of items contained in the <see cref="DictionaryRepository{TItem, TKey, TValue}" />
        /// </summary>
        public int Count => this.dictionary.Count;

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
        ///     Add the specified <paramref name="key" /> and <paramref name="value" /> to the
        ///     <see cref="DictionaryRepository{TItem, TKey, TValue}" />.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(TKey key, TValue value)
        {
            this.dictionary.Add(key, value);
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

        /// <summary>
        ///     Receives data to save.
        /// </summary>
        /// <returns>Data to save.</returns>
        protected override object GetDataToSave()
        {
            return new SortedDictionary<TKey, TValue>(this.dictionary);
        }

        /// <summary>
        ///     Loads data from file.
        /// </summary>
        /// <param name="fileName">The name of the file from which the data will be loaded.</param>
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

    public abstract class DictionaryRepository<TItem, TKey, TValue> : DictionaryRepository<TKey, TValue>,
        IEnumerable<TItem>
    {
        /// <summary>
        ///     Return an enumerator that iterates through the <see cref="DictionaryRepository{TItem, TKey, TValue}" />.
        /// </summary>
        public IEnumerator<TItem> GetEnumerator()
        {
            return this.dictionary.Select(keyValuePair => this.KeyValuePair2Item(keyValuePair))
                .GetEnumerator();
        }

        /// <summary>
        ///     Return an enumerator that iterates through the <see cref="DictionaryRepository{TItem, TKey, TValue}" />.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        ///     Adds item dictionary.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(TItem item)
        {
            var keyValuePair = this.Item2KeyValuePair(item);
            this.dictionary[keyValuePair.Key] = keyValuePair.Value;
        }

        /// <summary>
        ///     Adds items the dictionary.
        /// </summary>
        /// <param name="items">Items to add.</param>
        public void AddRange(IEnumerable<TItem> items)
        {
            foreach (TItem item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        ///     Convert outer data item type to inner.
        /// </summary>
        /// <param name="item">Outer data item type to convert.</param>
        /// <returns>Inner data item type.</returns>
        protected abstract KeyValuePair<TKey, TValue> Item2KeyValuePair(TItem item);

        /// <summary>
        ///     Convert inner data item type to outer.
        /// </summary>
        /// <param name="keyValuePair">Inner data item type to convert.</param>
        /// <returns>Outer data item type.</returns>
        protected abstract TItem KeyValuePair2Item(KeyValuePair<TKey, TValue> keyValuePair);
    }
}