using System;
using System.Collections.Generic;
using System.Linq;

namespace SecretSanta.Extensions
{
    public static class Extensions
    {
        public static Random Random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Trims all strings and returns all strings with a value.
        /// </summary>
        public static IEnumerable<String> TrimAndRemoveEmpities(this IEnumerable<String> enumerable)
        {
            foreach(String original in enumerable)
            {
                String newString = original?.Trim();
                if (!String.IsNullOrEmpty(newString))
                    yield return newString;
            }
        }

        /// <summary>
        /// Selects a random item from the list.
        /// </summary>
        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default(T);
            var i = Random.Next(0, list.Count);
            return list[i];
        }

        /// <summary>
        /// Determines whether the list is all unique or there are duplications.
        /// </summary>
        public static (bool AreUnique, T Duplicant) AreAllUnique<T>(this IEnumerable<T> enumerable)
        {
            var set = new HashSet<T>();
            foreach (var item in enumerable)
                if (!set.Add(item))
                    return (AreUnique: false, Duplicant: item);

            return (AreUnique: true, Duplicant: default(T));
        }
    }
}