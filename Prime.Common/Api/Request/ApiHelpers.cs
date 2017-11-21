using System;
using System.Diagnostics;
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
            if (limiter.IsSafe(context))
                return;

            limiter.Limit();
        }
    }
}