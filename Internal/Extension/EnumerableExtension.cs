﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace UniFramework.Extension
{

    public static class EnumerableExtension
    {
        public static IEnumerable<t> Randomize<t>(this IEnumerable<t> target)
        {
            Random r = new Random();

            return target.OrderBy(x => (r.Next()));
        }

        // public static IEnumerable<T> Empty<T> ()
        // {
        // 	return new T[0];
        // }

        // public static IEnumerable<T> LazyEach<T> (this IEnumerable<T> source, Action<T> fn)
        // {
        // 	foreach (var item in source) {
        // 		fn.Invoke (item);

        // 		yield return item;
        // 	}
        // }

        // public static void Each<T> (this IEnumerable<T> source, Action<T> fn)
        // {
        // 	foreach (var item in source) {
        // 		fn.Invoke (item);
        // 	}
        // }

        // public static void Each<T> (this IEnumerable<T> source, Action<T, int> fn)
        // {
        // 	int index = 0;

        // 	foreach (T item in source) {
        // 		fn.Invoke (item, index);
        // 		index++;
        // 	}
        // }
    }
}
