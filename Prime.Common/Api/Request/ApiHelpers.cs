using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Common
{
    public static class ApiHelpers
    {
        public static async Task<ApiResponse<T>> WrapExceptionAsync<T>(Func<Task<T>> t, string name, INetworkProvider provider, NetworkProviderContext context, Action<T> afterSuccess = null)
        {
            if (t == null)
                return new ApiResponse<T>("Not implemented");

            try
            {
                EnterRate(provider, context);
                var sw = new Stopwatch();
                sw.Start();
                context.L.Trace("Api: " + provider.Title + " " + name);
                var response = await t.Invoke().ConfigureAwait(false);
                context.L.Trace("Api finished @ " + sw.ToElapsed() + " : " + provider.Title + " " + name);
                if (response!=null)
                    afterSuccess?.Invoke(response);
                return new ApiResponse<T>(response);
            }
            catch (ApiResponseException ae)
            {
                context.L.Error("Api Error: " + provider.Title + " " + name + ": " + ae.Message);
                return new ApiResponse<T>(ae.Message);
            }
            catch (Exception e)
            {
                context.L.Error("Api Error: " + provider.Title + " " + name + ": " + e.Message);
                return new ApiResponse<T>(e);
            }
        }

        public static void EnterRate(INetworkProvider provider, NetworkProviderContext context)
        {
            var limiter = provider.RateLimiter;
            if (limiter.IsSafe())
                return;

            limiter.Limit();
        }

        public static IEnumerable<KeyValuePair<string, string>> DecodeUrlEncodedBody(string body)
        {
            return body.Split(new[] {"&"}, StringSplitOptions.RemoveEmptyEntries).Select(x =>
            {
                var parts = x.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    throw new FormatException("Invalid format of post data");

                return new KeyValuePair<string, string>(parts[0], parts[1]);
            });
        }
    }
}