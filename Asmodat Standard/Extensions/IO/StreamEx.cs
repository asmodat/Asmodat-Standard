using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandard.Extensions.IO
{
    public static class StreamEx
    {
        public static MemoryStream ToMemoryBlob(this Stream stream, int maxLength, int bufferSize = 128 * 1024)
        {
            int read = 0;
            int total = 0;
            var buffer = new byte[bufferSize];
            var result = new byte[maxLength];
            while((read = stream.Read(buffer, 0, bufferSize)) > 0)
            {
                buffer.CopyTo(result, total);
                total += read;
            }

            return new MemoryStream(total == maxLength ? result : result.SubArray(0, total));
        }

        public static byte[] ToArray(this Stream stream, int bufferSize = 256 * 1024)
        {
            int read = 0;
            var buffer = new byte[bufferSize];
            var result = new List<byte>();
            while ((read = stream.Read(buffer, 0, bufferSize)) > 0)
                result.AddRange(buffer.SubArray(0, read));

            return result.ToArray();
        }

        public static MemoryStream CopyToMemoryStream(this Stream stream, int bufferSize = 256 * 1024)
            => new MemoryStream(stream.ToArray(bufferSize: bufferSize));
    }
}
