using System;

namespace Prime.Base.Misc.Utils
{
    public static class ByteUtils
    {
        /// <summary>
        /// Creates new byte array which has it's size put into the first 4 bytes of the array.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] PrefixBufferSize(byte[] buffer)
        {
            var uint32Size = sizeof(UInt32);
            var bufferLength = (UInt32)buffer.Length;
            var newBuffer = new byte[uint32Size + bufferLength];
            var bufferLengthBytes = BitConverter.GetBytes(bufferLength);

            Buffer.BlockCopy(bufferLengthBytes, 0, newBuffer, 0, uint32Size);
            Buffer.BlockCopy(buffer, 0, newBuffer, uint32Size, (newBuffer.Length - uint32Size) * sizeof(byte));

            return newBuffer;
        }

        public static UInt32 ExtractDataSize(byte[] buffer)
        {
            var messageSizeBytes = new byte[sizeof(UInt32)];
            Buffer.BlockCopy(buffer, 0, messageSizeBytes, 0, sizeof(UInt32));

            var messageSize = BitConverter.ToUInt32(messageSizeBytes, 0);
            if (messageSize == 0) 
                throw new InvalidOperationException("Extracetd size of message equals to 0.");

            return messageSize;
        }
    }
}