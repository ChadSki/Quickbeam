using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quickbeam.Low.ByteArray;

namespace UnitTests
{
    [TestClass]
    public class FileByteArrayTest
    {
        private static readonly IByteArrayBuilder Builder = new FileByteArrayBuilder("bloodgulch.map");

        [TestMethod]
        public void TestReadBytes()
        {
            var byteArray = Builder.CreateByteArray(0, 4);
            var bytes = byteArray.ReadBytes(0, 4);
            Assert.IsTrue(bytes.SequenceEqual(new byte[] { 0x64, 0x61, 0x65, 0x68 }));
        }

        [TestMethod]
        public void TestReadUInt32()
        {
            var byteArray = Builder.CreateByteArray(0, 128);
            var num = byteArray.ReadUInt32(4);
            Assert.AreEqual(num, (UInt32)7);
        }

        [TestMethod]
        public void TestReadAscii()
        {
            var byteArray = Builder.CreateByteArray(0, 128);
            var ascii = byteArray.ReadAsciiz(32);
            Assert.AreEqual(ascii, "bloodgulch");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "offset + length too large: not allowed to read outside of MapAccess range")]
        public void TestAccessDenied()
        {
            var byteArray = Builder.CreateByteArray(0, 128);
            byteArray.ReadBytes(120, 120);
            Assert.Fail();
        }
    }
}
