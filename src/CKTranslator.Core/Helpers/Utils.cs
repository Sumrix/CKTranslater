using System;
using System.Collections.Generic;

namespace CKTranslator
{
    public static class Utils
    {
        /// <summary>
        ///     Adds the elements of the specified collection to the end of the <see cref="List{T}" />.
        ///     <para>Used in ModManager to initialize <see cref="MultiProcess" /> with both elements and collections.</para>
        /// </summary>
        /// <param name="list"><see cref="List{T}" /> at the end of which new items will be added.</param>
        /// <param name="collection">
        ///     The collection whose elements should be added to the end of the <see cref="List{T}" />
        /// </param>
        public static void Add<T>(this List<T> list, IEnumerable<T> collection)
        {
            list.AddRange(collection);
        }

        /// <summary>
        ///     Sorts the elements of a sequence in a particular direction according to dependencies.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="dependenciesSelector">Dependencies selector function.</param>
        /// <param name="throwOnCycle">
        ///     Determines whether an exception should be thrown if a circular dependency
        ///     is encountered.
        /// </param>
        /// <returns>An <see cref="IEnumerable{T}" /> whose elements are sorted according to dependencies.</returns>
        public static IEnumerable<TSource> OrderByTopology<TSource>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> dependenciesSelector,
            bool throwOnCycle = false)
        {
            var sorted = new List<TSource>();
            var visited = new HashSet<TSource>();

            foreach (TSource item in source)
            {
                Visit(item, visited, sorted, dependenciesSelector, throwOnCycle);
            }

            return sorted;

            static void Visit<T>(T item, ISet<T> visited, ICollection<T> sorted,
                Func<T, IEnumerable<T>> dependenciesSelector, bool throwOnCycle)
            {
                if (!visited.Contains(item))
                {
                    visited.Add(item);

                    foreach (T dep in dependenciesSelector(item))
                    {
                        Visit(dep, visited, sorted, dependenciesSelector, throwOnCycle);
                    }

                    sorted.Add(item);
                }
                else if (throwOnCycle && !sorted.Contains(item))
                {
                    throw new InvalidOperationException("Cyclic dependency found");
                }
            }
        }
    }
}