using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Core.Interfaces
{
    /// <summary>
    /// <typeparamref name="CollectionExtensions"/> static class, provides extension methods for <typeparamref name="Collection"/> types.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Checks if the specified predicate returns any results.
        /// </summary>
        /// <typeparam name="T">The Type of items in the collection.</typeparam>
        /// <param name="col">The collection.</param>
        /// <param name="predicate">The predicate to filter results.</param>
        /// <returns>True if the specified predicate returns any results.</returns>
        public static bool Exists<T>(this ICollection<T> col, Predicate<T> predicate)
        {
            return col.Where(new Func<T, bool>(predicate)).Any();
        }

        public static Dictionary<string, IList<string>> Pivot(this Dictionary<string, IEnumerable<string>> source)
        {
            if(source == null) throw new ArgumentNullException(nameof(source));

            var uniqueSourceValues = new HashSet<string>(source.SelectMany(x => x.Value));

            var target = new Dictionary<string, List<string>>();

            foreach (var targetKey in uniqueSourceValues)
            {
                var sourceKeys = source.Where(x => x.Value.Contains(targetKey)).Select(x=>x.Key);

                List<string> targetValue;

                if (target.ContainsKey(targetKey))
                {
                    targetValue = target[targetKey];
                }
                else
                {
                    targetValue = new List<string>();
                    target.Add(targetKey, targetValue);    
                }

                targetValue.AddRange(sourceKeys);
            }

            return target.ToDictionary<KeyValuePair<string, List<string>>, string, IList<string>>(pair => pair.Key, pair => pair.Value);
        }
    }
}
