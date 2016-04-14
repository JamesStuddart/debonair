using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debonair.Utilities.Extensions
{
    public static class ListExts
    {
        public static List<List<TEntity>> ChunkBy<TEntity>(this List<TEntity> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.Where(element => new HashSet<TKey>().Add(keySelector(element)));
        }
    }
}
