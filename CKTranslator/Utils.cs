using System;
using System.Collections.Generic;

namespace CKTranslator
{
    public static class Utils
    {
        public static void Add<T>(this List<T> list, IEnumerable<T> ie)
        {
            foreach (T item in ie)
            {
                list.Add(item);
            }
        }

        /// <summary>
        ///     Сортировка по ссылкам
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="dependencies"></param>
        /// <param name="throwOnCycle"></param>
        /// <returns></returns>
        public static IEnumerable<T> TSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies,
            bool throwOnCycle = false)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (T item in source)
            {
                Utils.Visit(item, visited, sorted, dependencies, throwOnCycle);
            }

            return sorted;
        }

        private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies,
            bool throwOnCycle)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (T dep in dependencies(item))
                {
                    Utils.Visit(dep, visited, sorted, dependencies, throwOnCycle);
                }

                sorted.Add(item);
            }
            else
            {
                if (throwOnCycle && !sorted.Contains(item))
                {
                    throw new Exception("Cyclic dependency found");
                }
            }
        }
    }
}