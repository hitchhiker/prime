using System.Net.Http;
using RestEase;

namespace plugins
{
    public class DebugDeserialiser : ResponseDeserializer
    {
        public override T Deserialize<T>(string content, HttpResponseMessage response, ResponseDeserializerInfo info)
        {
            // Consider caching generated XmlSerializers
            var serializer = new JsonResponseDeserializer();
            if (content.Contains("Error"))
                return default(T);

            return base.Deserialize<T>(content, response, info);
        }
    }
}