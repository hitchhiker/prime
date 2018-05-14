using System.IO;
using Newtonsoft.Json;
using Prime.Core;

namespace Prime.MessageServer.Data
{
    public class JsonDataProvider
    {
        private readonly Core.MessageServer _server;
        private readonly MessageTypeNameSerializationBinder _typeBinder;
        private readonly JsonSerializerSettings _settings;

        public JsonDataProvider(Core.MessageServer server)
        {
            _server = server;
            _typeBinder = server.TypeBinder;
            _settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = _typeBinder
            };
        }

        public int ReceiveBufferSize = 1024;
        
        public bool SendData<T>(IdentifiedClient identifiedClient, T data)
        {
            if (!identifiedClient.TcpClient.Connected)
                return false;

            var serializedData = Serialize(data);
            
            var dataString = serializedData.ToString();

            var dataBytes = dataString.GetBytes();
            identifiedClient.TcpClient.GetStream().Write(dataBytes, 0, dataBytes.Length);

            _server.L.Log("TCP sent: " + dataString);

            return true;
        }

        public object ReceiveData(Stream stream)
        {
            var buffer = new byte[ReceiveBufferSize];
            if (stream.CanRead)
                stream.Read(buffer, 0, buffer.Length);

            return buffer.GetString();
        }

        public TType Deserialize<TType>(object serializedData)
        {
            return JsonConvert.DeserializeObject<TType>(serializedData.ToString(), _settings);
        }

        public object Deserialize(object serializedData)
        {
            return JsonConvert.DeserializeObject(serializedData.ToString(), _settings);
        }

        public object Serialize<TType>(TType data)
        {
            return JsonConvert.SerializeObject(data, _settings);
        }
    }
}