﻿using System;
using System.Collections.Generic;

namespace CKTranslator.Core
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
            this IEnumerable<TSource> source,
            IEnumerable<int> sizes)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (sizes == null)
            {
                throw new ArgumentNullException(nameof(sizes));
            }

            return _();

            IEnumerable<IEnumerable<TSource>> _()
            {
                using var e = source.GetEnumerator();

                foreach (int size in sizes)
                {
                    yield return _();

                    IEnumerable<TSource> _()
                    {
                        for (int index = 0; index < size && e.MoveNext(); index++)
                        {
                            yield return e.Current;
                        }
                    }
                }
            }
        }

        public static bool Contains(this string text, string value, StringComparison comparsionType)
        {
            return text.IndexOf(text, comparsionType) >= 0;
        }

        public static IEnumerable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(
            this IEnumerable<TFirst> first, IEnumerable<TSecond> second)
        {
            using var enumerator1 = first.GetEnumerator();
            using var enumerator2 = second.GetEnumerator();

            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
                yield return (enumerator1.Current, enumerator2.Current);
            }
        }
    }
}