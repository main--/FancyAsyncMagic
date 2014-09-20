using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FancyAsyncMagic
{
    public class AsyncWriter
    {
        private readonly Stream _Stream;

        public AsyncWriter(Stream stream)
        {
            _Stream = stream;
        }

        private Task WriteBytes(byte[] data, CancellationToken token)
        {
            return _Stream.WriteAsync(data, 0, data.Length, token);
        }

        private async Task WriteBytesAndType(byte[] data, ElementType type, CancellationToken token, byte[] dataPrefix = null)
        {
            await WriteBytes(new byte[] { (byte)type }, token);
            if (dataPrefix != null)
                await WriteBytes(dataPrefix, token);
            await WriteBytes(data, token);
        }

        public Task Write(byte b, CancellationToken token)
        {
            return WriteBytesAndType(new byte[] { b }, ElementType.Byte, token);
        }

        public Task Write(Int32 i, CancellationToken token)
        {
            return WriteBytesAndType(BitConverter.GetBytes(i), ElementType.Int32, token);
        }

        public Task Write(Int64 i, CancellationToken token)
        {
            return WriteBytesAndType(BitConverter.GetBytes(i), ElementType.Int64, token);
        }

        public Task Write(UInt32 i, CancellationToken token)
        {
            return WriteBytesAndType(BitConverter.GetBytes(i), ElementType.UInt32, token);
        }

        public Task Write(UInt64 i, CancellationToken token)
        {
            return WriteBytesAndType(BitConverter.GetBytes(i), ElementType.UInt64, token);
        }

        public Task Write(byte[] data, CancellationToken token)
        {
            return WriteBytesAndType(data, ElementType.ByteArray, token, BitConverter.GetBytes(data.Length));
        }

        public Task Write(string s, CancellationToken token)
        {
            return WriteBytesAndType(Encoding.UTF8.GetBytes(s), ElementType.String, token, BitConverter.GetBytes(Encoding.UTF8.GetByteCount(s)));
        }
    }
}
