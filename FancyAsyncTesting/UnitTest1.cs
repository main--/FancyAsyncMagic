using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using FancyAsyncMagic;
using System.Threading;
using System.Net;

namespace FancyAsyncTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAllTheThings()
        {
            using (var stream = new MemoryStream())
            {
                var writer = new AsyncWriter(stream);
                var t = CancellationToken.None;
                writer.Write((Byte)42, t).Wait();
                writer.Write((Int32)42, t).Wait();
                writer.Write((Int64)42, t).Wait();
                writer.Write((UInt32)42, t).Wait();
                writer.Write((UInt64)42, t).Wait();
                writer.Write(new byte[] { }, t).Wait();
                writer.Write(new byte[] { 13, 37 }, t).Wait();
                writer.Write("#rekt", t).Wait();

                writer.Write("nope.avi", t).Wait();

                stream.Position = 0;

                var reader = new AsyncReader(stream);
                Assert.AreEqual<Byte>(42, reader.ReadByte(t).Result);
                Assert.AreEqual<Int32>(42, reader.ReadInt32(t).Result);
                Assert.AreEqual<Int64>(42, reader.ReadInt64(t).Result);
                Assert.AreEqual<UInt32>(42, reader.ReadUInt32(t).Result);
                Assert.AreEqual<UInt64>(42, reader.ReadUInt64(t).Result);
                Assert.IsTrue(reader.ReadByteArray(t).Result.Length == 0);
                Assert.IsTrue(new byte[] { 13, 37 }.SequenceEqual(reader.ReadByteArray(t).Result));
                Assert.AreEqual<string>("#rekt", reader.ReadString(t).Result);

                try
                {
                    reader.ReadByte(t).Wait();
                    Assert.IsTrue(false, "wtf");
                }
                catch (AggregateException e)
                {
                    Assert.IsInstanceOfType(e.InnerExceptions.Single(), typeof(ProtocolViolationException));
                }
            }
        }
    }
}
