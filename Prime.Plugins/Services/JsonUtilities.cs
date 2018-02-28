using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RestEase;

namespace Prime.Plugins.Services
{
    public static class JsonUtilities
    {
        /// <summary>
        /// Tries to deserialize JSON to specified type. Useful when endpoint returns different data schema.
        /// </summary>
        /// <typeparam name="TResponse">The default deserialization type of response.</typeparam>
        /// <typeparam name="TRequired">The type which deserialization should be tested to.</typeparam>
        /// <param name="response">The class which extension method is added to.</param>
        /// <param name="content">The output parameter where deserialized result will be stored.</param>
        /// <returns></returns>
        public static bool TryGetContent<TResponse, TRequired>(this Response<TResponse> response, out TRequired content) where TRequired : class
        {
            try
            {
                if (typeof(TResponse) == typeof(TRequired))
                {
                    content = response.GetContent() as TRequired;
                }
                else
                {
                    var json = response.StringContent;
                    var jsonObj = JsonConvert.DeserializeObject<TRequired>(json);

                    content = jsonObj;
                }

                return true;
            }
            catch (Exception)
            {
                content = null;
                return false;
            }
        }
    }
}
