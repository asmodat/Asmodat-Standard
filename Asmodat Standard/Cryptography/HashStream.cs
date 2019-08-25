using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AsmodatStandard.Cryptography
{
    public class HashStream : Stream
    {
        public Stream Source { get; }
        public long Offset { get; }

        public override long Length { get; }

        private long End => Offset + Length;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Position
        {
            get => Source.Position - Offset;
            set => throw new NotSupportedException();
        }

        private IncrementalHash IH { get; set; }

        public HashStream(Stream source, HashAlgorithmName? hashAlgorithm = null)
        {
            IH = IncrementalHash.CreateHash(hashAlgorithm ?? HashAlgorithmName.MD5);
            Offset = source.Position;
            Source = source;
        }

        public HashStream(Stream source, long length, HashAlgorithmName? hashAlgorithm = null)
        {
            IH = IncrementalHash.CreateHash(hashAlgorithm ?? HashAlgorithmName.MD5);
            Offset = source.Position;
            Length = length;
            Source = source;
        }

        public HashStream(Stream source, long offset, long length, bool seekToOffset = true, HashAlgorithmName? hashAlgorithm = null)
        {
            if (seekToOffset) source.Seek(offset, SeekOrigin.Begin);
            Offset = offset;
            Length = length;

            IH = IncrementalHash.CreateHash(hashAlgorithm ?? HashAlgorithmName.MD5);
        }

        public override int Read(byte[] array, int offset, int count)
        {
            if (Source.Position >= End) return 0;

            if (Source.Position + count > End)
                count = (int)(End - Source.Position);

            var result = Source.Read(array, offset, count);
            IH.AppendData(array, offset, result);

            return result;
        }

        public byte[] GetHashAndReset()
            => IH.GetHashAndReset();

        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
