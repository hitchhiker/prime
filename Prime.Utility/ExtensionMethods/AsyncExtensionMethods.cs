using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prime.Utility
{
    public static class AsyncExtensionMethods
    {
        /// <summary>
        /// https://stackoverflow.com/a/32554428/1318333
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        [Obsolete]
        public static Task ProcessTenAtOnce<T>(this IEnumerable<T> items, Func<T, Task> func)
        {
            var maxThread = new SemaphoreSlim(10);
            for (var i = 0; i < 115; i++)
            {
                maxThread.Wait();
                Task.Factory.StartNew(() => {  }).ContinueWith(task => maxThread.Release());
            }
            return Task.CompletedTask;
        }

        public static Task ForEachAsync<T>(this IEnumerable<T> sequence, Func<T, Task> action)
        {
            return Task.WhenAll(sequence.Select(action));
        }
    }
}
