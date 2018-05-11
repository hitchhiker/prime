using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Prime.Core
{
    public class MessageTypeNameSerializationBinder : ISerializationBinder
    {
        public readonly MessageTypeCatalogue TypeCatalogue;

        public MessageTypeNameSerializationBinder(ServerContext context)
        {
            TypeCatalogue = new MessageTypeCatalogue(context.Extensions);
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