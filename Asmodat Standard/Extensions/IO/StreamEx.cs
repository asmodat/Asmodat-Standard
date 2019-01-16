using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandard.Extensions.IO
{
    public static class StreamEx
    {
        public static Task<string> ToStringAsync(this Stream stream, Encoding encoding = null)
            => (encoding == null ?
            new StreamReader(stream) :
            new StreamReader(stream, encoding))
            .ReadToEndAsync();

        public static Task<string> ToStringAsync(this Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
            => (encoding == null ? 
            new StreamReader(stream,
                detectEncodingFromByteOrderMarks: detectEncodingFromByteOrderMarks) :
            new StreamReader(stream, encoding, 
                detectEncodingFromByteOrderMarks: detectEncodingFromByteOrderMarks))
            .ReadToEndAsync();

        public static MemoryStream ToMemoryBlob(this Stream stream, int maxLength, int bufferSize = 128 * 1024)
        {
            int read = 0;
            int total = 0;
            int remaining;
            var buffer = new byte[bufferSize];
            var result = new byte[maxLength];
            while((remaining = maxLength - total) > 0 && (read = stream.Read(buffer, 0, Math.Min(bufferSize, remaining))) > 0)
            {
                buffer.CopyTo(0, result, total, read);
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

        public static BufferedStream ToBufferedStream(this Stream stream, int bufferSize = 256 * 1024)
            => new BufferedStream(stream, bufferSize);

        public static StreamWriter ToStreamWriter(this Stream stream, Encoding encoding = null)
            => encoding == null ? new StreamWriter(stream) : new StreamWriter(stream, encoding);

        public static StreamWriter ToStreamWriter(this Stream stream, Encoding encoding, int bufferSize)
            => new StreamWriter(stream, encoding, bufferSize);
    }
}
