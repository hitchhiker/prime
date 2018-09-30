using System;
using Newtonsoft.Json.Serialization;
using Prime.Core;

namespace Prime.MessagingServer.Types
{
    public class MessageTypeNameSerializationBinder : ISerializationBinder
    {
        public readonly MessageTypeCatalogue TypeCatalogue;

        public MessageTypeNameSerializationBinder(PrimeContext context)
        {
            TypeCatalogue = new MessageTypeCatalogue(context);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            typeName = TypeCatalogue.GetSkip(serializedType);
            assemblyName = null;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            return TypeCatalogue.Get(typeName);
        }
    }
}