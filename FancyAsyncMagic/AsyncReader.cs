using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FancyAsyncMagic
{
    public class AsyncReader
    {
        private readonly Stream _Stream;

        public AsyncReader(Stream stream)
        {
            _Stream = stream;
        }

        private async Task<byte[]> ReadBytes(int len, CancellationToken token)
        {
            int pos = 0;
            var buf = new byte[len];
            do pos += await _Stream.ReadAsync(buf, pos, len - pos, token);
            while (pos != len);
            return buf;
        }

        public async Task<byte> ReadByte(CancellationToken token)
        {
            var packet = await ReadAndValidateType(sizeof(byte), ElementType.Byte, token);
            return packet[1];
        }

        private async Task<byte[]> ReadAndValidateType(int len, ElementType expected, CancellationToken token)
        {
            var packet = await ReadBytes(len + 1, token);
            var type = (ElementType)packet[0];
            if (type != expected)
                throw new ProtocolViolationException(String.Format("Expected {0}, but got {1}", expected, type));
            return packet;
        }

        public async Task<Int32> ReadInt32(CancellationToken token)
        {
            return BitConverter.ToInt32(await ReadAndValidateType(sizeof(Int32), ElementType.Int32, token), 1);
        }

        public async Task<Int64> ReadInt64(CancellationToken token)
        {
            return BitConverter.ToInt64(await ReadAndValidateType(sizeof(Int64), ElementType.Int64, token), 1);
        }

        public async Task<UInt32> ReadUInt32(CancellationToken token)
        {
            return BitConverter.ToUInt32(await ReadAndValidateType(sizeof(UInt32), ElementType.UInt32, token), 1);
        }

        public async Task<UInt64> ReadUInt64(CancellationToken token)
        {
            return BitConverter.ToUInt64(await ReadAndValidateType(sizeof(UInt64), ElementType.UInt64, token), 1);
        }

        public async Task<byte[]> ReadByteArray(CancellationToken token)
        {
            var length = BitConverter.ToInt32(await ReadAndValidateType(sizeof(Int32), ElementType.ByteArray, token), 1);
            return await ReadBytes(length, token);
        }

        public async Task<string> ReadString(CancellationToken token)
        {
            var length = BitConverter.ToInt32(await ReadAndValidateType(sizeof(Int32), ElementType.String, token), 1);
            return Encoding.UTF8.GetString(await ReadBytes(length, token));
        }
    }
}
