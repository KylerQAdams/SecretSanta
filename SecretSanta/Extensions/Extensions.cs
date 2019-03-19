using System;
using System.Collections.Generic;

namespace SecretSanta.Extensions
{
    public static class Extensions
    {
        public static Random Random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Trims all strings and returns all strings with a value.
        /// </summary>
        public static IEnumerable<String> TrimAndRemoveEmpities(this IEnumerable<String> ienumerable)
        {
            foreach(String original in ienumerable)
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
    }
}