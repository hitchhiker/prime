using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core
{
    public static class StreamExtensionMethods
    {
        public static string ToX2String(this IEnumerable<byte> b)
        {
            var o = String.Empty;
            foreach (byte b1 in b)
            {
                o += b1.ToString("X2");
            }
            return o;
        }

        public static string DecodeUtf32(this byte[] buffer)
        {
            var count = Array.IndexOf<byte>(buffer, 0, 0);
            if (count < 0) count = buffer.Length;
            return Encoding.UTF32.GetString(buffer, 0, count);
        }

        private const int DefaultBufferSize = short.MaxValue; // +32767

        /// <summary>
        /// https://stackoverflow.com/a/4139427
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void CopyTo(this Stream input, Stream output)
        {
            input.CopyTo(output, DefaultBufferSize);
            return;
        }

        public static void CopyTo(this Stream input, Stream output, int bufferSize)
        {
            if (!input.CanRead) throw new InvalidOperationException("input must be open for reading");
            if (!output.CanWrite) throw new InvalidOperationException("output must be open for writing");

            byte[][] buf = {new byte[bufferSize], new byte[bufferSize]};
            int[] bufl = {0, 0};
            int bufno = 0;
            IAsyncResult read = input.BeginRead(buf[bufno], 0, buf[bufno].Length, null, null);
            IAsyncResult write = null;

            while (true)
            {

                // wait for the read operation to complete
                read.AsyncWaitHandle.WaitOne();
                bufl[bufno] = input.EndRead(read);

                // if zero bytes read, the copy is complete
                if (bufl[bufno] == 0)
                {
                    break;
                }

                // wait for the in-flight write operation, if one exists, to complete
                // the only time one won't exist is after the very first read operation completes
                if (write != null)
                {
                    write.AsyncWaitHandle.WaitOne();
                    output.EndWrite(write);
                }

                // start the new write operation
                write = output.BeginWrite(buf[bufno], 0, bufl[bufno], null, null);

                // toggle the current, in-use buffer
                // and start the read operation on the new buffer.
                //
                // Changed to use XOR to toggle between 0 and 1.
                // A little speedier than using a ternary expression.
                bufno ^= 1; // bufno = ( bufno == 0 ? 1 : 0 ) ;
                read = input.BeginRead(buf[bufno], 0, buf[bufno].Length, null, null);

            }

            // wait for the final in-flight write operation, if one exists, to complete
            // the only time one won't exist is if the input stream is empty.
            if (write != null)
            {
                write.AsyncWaitHandle.WaitOne();
                output.EndWrite(write);
            }

            output.Flush();

            // return to the caller ;
            return;
        }


        public static async Task CopyToAsync(this Stream input, Stream output)
        {
            await input.CopyToAsync(output, DefaultBufferSize);
        }

        public static async Task CopyToAsync(this Stream input, Stream output, int bufferSize)
        {
            if (!input.CanRead) throw new InvalidOperationException("input must be open for reading");
            if (!output.CanWrite) throw new InvalidOperationException("output must be open for writing");

            byte[][] buf = {new byte[bufferSize], new byte[bufferSize]};
            int[] bufl = {0, 0};
            int bufno = 0;
            var read = input.ReadAsync(buf[bufno], 0, buf[bufno].Length);
            Task write = null;

            while (true)
            {

                await read;
                bufl[bufno] = read.Result;

                // if zero bytes read, the copy is complete
                if (bufl[bufno] == 0)
                {
                    break;
                }

                // wait for the in-flight write operation, if one exists, to complete
                // the only time one won't exist is after the very first read operation completes
                if (write != null)
                {
                    await write;
                }

                // start the new write operation
                write = output.WriteAsync(buf[bufno], 0, bufl[bufno]);

                // toggle the current, in-use buffer
                // and start the read operation on the new buffer.
                //
                // Changed to use XOR to toggle between 0 and 1.
                // A little speedier than using a ternary expression.
                bufno ^= 1; // bufno = ( bufno == 0 ? 1 : 0 ) ;
                read = input.ReadAsync(buf[bufno], 0, buf[bufno].Length);

            }

            // wait for the final in-flight write operation, if one exists, to complete
            // the only time one won't exist is if the input stream is empty.
            if (write != null)
            {
                await write;
            }

            output.Flush();

            // return to the caller ;
        }
    }
}
