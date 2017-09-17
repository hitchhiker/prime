﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Prime.Utility;

namespace Prime.Core
{
    public static class ApiHelpers
    {
        public static async Task<ApiResponse<T>> WrapException<T>(Func<Task<T>> t, string name, INetworkProvider provider, NetworkProviderContext context = null)
        {
            if (t == null)
                return new ApiResponse<T>("Not implemented");

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                context.L.Trace("Api: " + provider.Network + " " + name);
                var response = await t.Invoke();
                context.L.Trace("Api finished @ " + sw.ToElapsed() + " : " + provider.Network + " " + name);
                return new ApiResponse<T>(response);
            }
            catch (ApiResponseException ae)
            {
                return new ApiResponse<T>(ae.Message);
            }
            catch (Exception e)
            {
                return new ApiResponse<T>(e);
            }
        }
    }
}