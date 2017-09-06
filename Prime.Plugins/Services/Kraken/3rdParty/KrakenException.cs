using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KrakenApi
{
    [Serializable]
    public class KrakenException : Exception
    {
        public ResponseBase Response { get; }

        public KrakenException()
        {
        }

        public KrakenException(string message) : base(message)
        {
        }

        public KrakenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public KrakenException(string message, ResponseBase response) : base(message)
        {
            Response = response;
        }

        protected KrakenException(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("Response", Response);
        }
    }
}