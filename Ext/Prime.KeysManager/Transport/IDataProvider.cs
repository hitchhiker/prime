using System;
using System.IO;
using System.Net.Sockets;

namespace Prime.KeysManager.Transport
{
    public interface IDataProvider
    {
        /// <summary>
        /// Sends data to specified client.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <returns>False if client is disconnected, true if data is sent.</returns>
        bool SendData<T>(TcpClient client, T data);
        
        object ReceiveData(Stream stream);
        
        TType Deserialize<TType>(object serializedData);
        object Deserialize(object serializedData, Type targetType);
        
        object Serialize<TType>(TType data);
    }
}