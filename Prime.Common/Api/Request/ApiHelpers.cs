using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Common
{
    public static class ApiHelpers
    {
        public static async Task<ApiResponse<T>> WrapException<T>(Func<Task<T>> t, string name, INetworkProvider provider, NetworkProviderContext context)
        {
            if (t == null)
                return new ApiResponse<T>("Not implemented");

            try
            {
                EnterRate(provider, context);
                var sw = new Stopwatch();
                sw.Start();
                context.L.Trace("Api: " + provider.Network + " " + name);
                var response = await t.Invoke();
                context.L.Trace("Api finished @ " + sw.ToElapsed() + " : " + provider.Network + " " + name);
                return new ApiResponse<T>(response);
            }
            catch (ApiResponseException ae)
            {
                context.L.Error("Api Error: " + provider.Network + " " + name + ": " + ae.Message);
                return new ApiResponse<T>(ae.Message);
            }
            catch (Exception e)
            {
                context.L.Error("Api Error: " + provider.Network + " " + name + ": " + e.Message);
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