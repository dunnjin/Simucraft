using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Simucraft.Server.Common
{
    public static class GenericExtensions
    {
        public static string ToJson<TSource>(this TSource source) =>
            JsonConvert.SerializeObject(source);

        public static TSource FromJson<TSource>(this string json) =>
            JsonConvert.DeserializeObject<TSource>(json);

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> source)
        {
            foreach (var element in source)
                collection.Add(element);
        }
        public static IEnumerable<string> RegexMatches(this string source, string regex) => Regex
            .Matches(source, regex)
            .OfType<Match>()
            .Where(m => m.Captures.Count > 0)
            .Select(m => string.Concat(m.Captures
                .OfType<Capture>()
                .Select(c => c.Value)))
            .ToList();

        public static int Wrap(this int source, int min, int max) =>
            Math.Abs(source) > Math.Abs(max) ? min : source;

        public static StringBuilder AppendWhen(this StringBuilder stringBuilder, string value, Func<string, bool> expression)
        {
            if (expression(value))
                stringBuilder.Append(value);

            return stringBuilder;
        }

        public static int ToHighestMultiple(this int value, int factor) =>
            (int)Math.Ceiling(value / (double)factor) * factor;

        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> collection)
        {
            foreach (var element in collection)
                queue.Enqueue(element);
        }
    }
}
