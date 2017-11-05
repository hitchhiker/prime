using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Utility;

namespace Prime.Common
{
    public abstract class ProviderCache<T,T2> : CacheDictionary<T, T2>
    {
        protected ProviderCache(TimeSpan expirationSpan) : base(expirationSpan)
        {
        }

        public async Task<T2> TryAsync(T provider, Func<Task<T2>> pull)
        {
            var task = new Task<T2>(() =>
            {
                return Try(provider, k => AsyncContext.Run(pull.Invoke));
            });

            task.Start();
            return await task;
        }
    }
}