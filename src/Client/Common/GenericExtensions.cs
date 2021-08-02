using BlazorInputFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Simucraft.Client.Common
{
    public static class GenericExtensions
    {
        public static async Task<byte[]> ToByteArrayAsync(this IFileListEntry fileListEntry)
        {
            using (var memoryStream = new MemoryStream())
            {
                await fileListEntry.Data.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static int ToHighestMultiple(this double value, int factor) =>
            (int)Math.Ceiling(value / (double)factor) * factor; 

        public static string Truncate(this string source, int length) =>
            source?.Length > length ? source.Substring(0, length) : source;

        public static string TruncateWithTrail(this string source, int length)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            var truncatedString = source.Truncate(length);
            if (truncatedString.Length < source.Length)
                return $"{truncatedString}...";

            return truncatedString;
        }

        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int size) => source
            .Select((x, i) =>
                new
                {
                    Index = i,
                    Value = x,
                })
            .GroupBy(s => s.Index / size)
            .Select(s => s
                .Select(v => v.Value)
                .ToList());

        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> collection)
        {
            foreach (var element in collection)
                source.Add(element);
        }

        public static string ToJson<TSource>(this TSource source) =>
            JsonConvert.SerializeObject(source);

        public static TSource FromJson<TSource>(this string json) =>
            JsonConvert.DeserializeObject<TSource>(json);

        public static TSource Copy<TSource>(this TSource source) =>
            source.ToJson().FromJson<TSource>();

        public static IEnumerable<string> RegexMatches(this string source, string regex) => Regex
            .Matches(source, regex)
            .OfType<Match>()
            .Where(m => m.Captures.Count > 0)
            .Select(m => string.Concat(m.Captures
                .OfType<Capture>()
                .Select(c => c.Value)))
            .ToList();

        public static int Clamp(this int source, int min, int max) =>
            Math.Min(Math.Max(source, min), max);

        public static int Wrap(this int source, int min, int max) =>
            Math.Abs(source) > Math.Abs(max) ? min : source;

        public static string GetDisplayName(this Enum source) => source
            .GetType()
            .GetCustomAttribute<DisplayAttribute>()
           ?.Name ?? source.ToString();

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector) => source
            .GroupBy(selector)
            .Select(g => g.First());
    }
}
